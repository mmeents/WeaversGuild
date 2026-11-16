using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;
using Weavers.Core.Handlers.AssetArchive;

namespace TheLoomApp.Extensions {
  public static class PlanListViewExt {

    /// <summary>One-time setup: columns, details view, groups, double buffering.</summary>
    public static void SetupPlanListView(this ListView lv) {
      lv.View = View.Details;
      lv.FullRowSelect = true;
      lv.HideSelection = false;
      lv.ShowGroups = true;
      lv.MultiSelect = false;
      lv.Columns.Clear();
      lv.Columns.Add("", 28);
      lv.Columns.Add("Size", 80, HorizontalAlignment.Right);
      lv.Columns.Add("Source", 320);
      lv.Columns.Add("Target", 320);
      lv.Columns.Add("Info", 260);

      // ListView.DoubleBuffered is protected; set it once so 1-2s refreshes don't flicker.
      typeof(Control).GetProperty("DoubleBuffered",
        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(lv, true);

    }

    /// <summary>
    /// Re-renders the board, keeping the current selection (by command Id) and scroll position.
    /// Item Tag = CommandRow (or PlanRow for the empty-plan placeholder); Group Tag = PlanRow.
    /// </summary>
    public static void RenderPlanBoard(this ListView lv, PlanBoard board, int perPlanLimit) {
      var selectedKey = lv.SelectedItems.Count > 0 ? lv.SelectedItems[0].Name : null;
      var topKey = lv.TopItem?.Name;

      lv.BeginUpdate();
      try {
        lv.Items.Clear();
        lv.Groups.Clear();

        foreach (var p in board.Plans) {
          var group = new ListViewGroup($"plan{p.Id}", PlanHeader(p)) { Tag = p };
          lv.Groups.Add(group);

          board.CommandsByPlan.TryGetValue(p.Id, out var cmds);
          if (cmds is null || cmds.Count == 0) {
            // ListView hides empty groups, so a plan with nothing to copy needs a row to be seen.
            lv.Items.Add(new ListViewItem(new[] { "", "", "(no commands)", "", "" }, group) { Name = $"p{p.Id}", Tag = p });
            continue;
          }

          foreach (var c in cmds) {
            var item = new ListViewItem(new[] {
              Glyph(c.Status), FormatBytes(c.Bytes), c.SourceRelPath, c.TargetRelPath, Info(c),
            }, group) { Name = $"c{c.Id}", Tag = c };
            if (c.Status == (byte)AssetCommandStatus.Failed) 
              item.ForeColor = Color.Firebrick;
            else if (c.Status == (byte)AssetCommandStatus.Done) 
              item.ForeColor = SystemColors.GrayText;
            lv.Items.Add(item);
          }

          if (p.CommandCount > cmds.Count) {
            lv.Items.Add(new ListViewItem(new[] {
              "", "", $"… {p.CommandCount - cmds.Count:N0} more (showing first {perPlanLimit:N0} by priority)", "", "",
            }, group) { 
              Name = $"m{p.Id}", 
              Tag = p, 
              ForeColor = SystemColors.GrayText 
            });
          }
        }

        if (selectedKey is not null && lv.Items.ContainsKey(selectedKey)) {
          var sel = lv.Items[selectedKey]!;
          sel.Selected = true;
          sel.Focused = true;
        } else { 
          if (lv.Items.Count > 0) {
            lv.Items[0].Selected = true;
            lv.Items[0].Focused = true;
          }
        }

        if (topKey is not null && lv.Items.ContainsKey(topKey)) {
          lv.TopItem = lv.Items[topKey];
        }
      } finally {
        lv.EndUpdate();
      }
    }

    /// <summary>Text for the summary box: the selected row's plan, then the command if one is selected.</summary>
    public static string DescribeSelection(this ListView lv) {
      if (lv.SelectedItems.Count == 0) return "";
      var item = lv.SelectedItems[0];
      var plan = item.Group?.Tag as PlanRow;
      if (plan is null) return "";

      var sb = new StringBuilder();
      sb.AppendLine(PlanHeader(plan));
      sb.AppendLine();
      sb.AppendLine(plan.ResultSummary ?? plan.PlanSummary ?? "(no summary)");

      if (item.Tag is CommandRow c) {
        sb.AppendLine();
        sb.AppendLine($"── Command {c.Id} ──");
        sb.AppendLine($"Status:   {(AssetCommandStatus)c.Status}   Attempts: {c.Attempts}");
        sb.AppendLine($"Source:   {c.SourceRelPath}");
        sb.AppendLine($"Target:   {c.TargetRelPath}");
        sb.AppendLine($"Size:     {FormatBytes(c.Bytes)}");
        if (c.ClaimedBy is not null) sb.AppendLine($"Claimed:  {c.ClaimedBy}");
        if (c.NotBeforeUtc is not null) sb.AppendLine($"Retry at: {c.NotBeforeUtc.Value.ToLocalTime():g}");
        if (!string.IsNullOrEmpty(c.LastError)) sb.AppendLine($"Error:    {c.LastError}");
      }
      return sb.ToString().Replace("\n", Environment.NewLine).Replace("\r\r", "\r");
    }

    private static string PlanHeader(PlanRow p) {
      var pct = p.CommandCount == 0 ? 100 : (int)(100.0 * (p.DoneCnt + p.SkippedCnt) / p.CommandCount);
      var sb = new StringBuilder($"Plan {p.Id} · {p.PolicyName} · {(AssetPlanStatus)p.Status} · ");
      sb.Append($"{p.CommandCount:N0} files, {FormatBytes(p.Bytes)}");
      if (p.Status != (byte)AssetPlanStatus.Draft) {
        sb.Append($" · {pct}% ({FormatBytes(p.DoneBytes)} done)");
        if (p.ClaimedCnt > 0) sb.Append($" · {p.ClaimedCnt} in flight");
        if (p.FailedCnt > 0) sb.Append($" · {p.FailedCnt} failed");
      }
      return sb.ToString();
    }

    private static string Glyph(byte status) => (AssetCommandStatus)status switch {
      AssetCommandStatus.Pending => "·",
      AssetCommandStatus.Claimed => "⟳",
      AssetCommandStatus.Done => "✓",
      AssetCommandStatus.Failed => "✗",
      AssetCommandStatus.Skipped => "–",
      _ => "?",
    };

    private static string Info(CommandRow c) {
      if (c.Status == (byte)AssetCommandStatus.Failed || (c.Attempts > 0 && c.LastError is not null)) {
        var retry = c.NotBeforeUtc is null ? "" : $" (try {c.Attempts}, next {c.NotBeforeUtc.Value.ToLocalTime():t})";
        return (c.LastError ?? "failed") + retry;
      }
      if (c.Status == (byte)AssetCommandStatus.Claimed) return c.ClaimedBy ?? "";
      return c.TargetRelPath.Contains('~') ? "renamed (collision)" : "";
    }

    private static string FormatBytes(long b) =>
      b >= 1L << 30 ? $"{b / (double)(1L << 30):N1} GB" :
      b >= 1L << 20 ? $"{b / (double)(1L << 20):N1} MB" :
      $"{b / 1024.0:N0} KB";
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Weavers.Core.Handlers.Builds;

namespace Weavers.Core.Extensions {
  public static class RealmPublishingExt {
    public static string WriteFolderEntry(this FolderEntryClass folder, string outputFolder) {
      if (folder == null) {
        return "";
      }
      var filePath = Path.Combine(outputFolder, folder.File);
      var json = JsonSerializer.Serialize(folder, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(filePath, json);

      return filePath;
    }

    public static string WriteIndexHtml(this FolderEntryClass folder, string outputFolder) {
      if (folder == null) {
        return "";
      }
      var filePath = Path.Combine(outputFolder, "index.html");
      var htmlContent = $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <title>{{folder.Name}}</title>
            <meta name="description" content="Machine-woven fiction from the Weavers Guild.">
            <link rel="stylesheet" href="loom.css">
        </head>
        <body>

        <div class="wrap">

            <header class="masthead">
                <a class="mono" href="../../default.html">&larr; go back</a>
                <button class="theme-toggle" id="theme" type="button"></button>
            </header>

            <div class="realm-head">
                <p class="mono" id="eyebrow">Weavers Guild</p>
                <h1 id="title">Threadspace</h1>
                <p class="mono" id="tally"></p>
            </div>

            <div class="filters" id="filters" hidden></div>

            <main class="shelf" id="shelf">
                <p class="note mono">Loading the shelf…</p>
            </main>

        </div>

        <script src="loom.js"></script>
        <script>
        mountThemeToggle(document.getElementById("theme"));

        var shelf = document.getElementById("shelf");        
        var allRealms = [];
        var folder = [];
        var activeRealm = "all";
        
        loadJSON("index.json")
          .then(function (data) {
            folder = data;
            allRealms = (data.realms || []);            
            render();
          })
          .catch(function (err) {
            shelf.innerHTML =
              '<div class="note"><strong>No shelf yet.</strong><br>' +
              'Expected <code>index.json</code>. ' +
              'If you opened this file directly, serve the folder over http — ' +
              '<code>fetch</code> is blocked on <code>file'+'://</code>.' +
              '<br><br><span class="mono">' + escapeHtml(err.message) + '</span></div>';
          });

        function realms() {
          var seen = [];
          allRealms.forEach(function (s) {
            if (s.id && seen.indexOf(s.id) === -1) seen.push(s.id);
          });
          return seen;
        }        

        function render() {
          var list = allRealms;
          document.getElementById("title").textContent = (folder.name || "Threadspace");
          shelf.innerHTML = "";

          if (!list.length) {
            shelf.innerHTML = '<div class="note"><strong>Nothing woven here yet.</strong><br>' +
              'Export a story to <code>stories/' + escapeHtml(activeRealm) +
              '/</code> and rebuild the index.</div>';
            return;
          }

         list.forEach(function (s) {
          var a = document.createElement("a");
          a.className = "entry";
          a.href = "realm.html?s=" + encodeURIComponent(s.file);

          if (s.coverImage) {
            var img = document.createElement("img");
            img.className = "thumb";
            img.src = s.coverImage;
            img.alt = "";
            a.appendChild(img);
          } else {
            a.appendChild(draftEl(s.file, 8));
          }

          var body = document.createElement("div");
          var h = document.createElement("h2");
          h.textContent = s.name;
          body.appendChild(h);

          if (s.blurb) {
            var p = document.createElement("p");
            p.className = "blurb";
            p.textContent = s.blurb;
            body.appendChild(p);
          }

          a.appendChild(body);
          shelf.appendChild(a);
        });
        }
        </script>
        </body>
        </html>
        """;
      File.WriteAllText(filePath, htmlContent);
      return filePath;
    }

    public static string WriteLoomJs(this FolderEntryClass folder, string outputFolder) {
      if (folder == null) {
        return "";
      }
      var filePath = Path.Combine(outputFolder, "loom.js");
      var jsContent = """
        (function initTheme() {
          var saved = null;
          try { saved = localStorage.getItem("loom-theme"); } catch (e) {}
          if (saved) document.documentElement.setAttribute("data-theme", saved);
        })();

        function mountThemeToggle(el) {
          if (!el) return;
          function label() {
            var t = document.documentElement.getAttribute("data-theme");
            if (!t) {
              t = matchMedia("(prefers-color-scheme: dark)").matches ? "indigo" : "linen";
            }
            el.textContent = t === "indigo" ? "linen" : "indigo";
          }
          el.addEventListener("click", function () {
            var cur = document.documentElement.getAttribute("data-theme");
            if (!cur) {
              cur = matchMedia("(prefers-color-scheme: dark)").matches ? "indigo" : "linen";
            }
            var next = cur === "indigo" ? "linen" : "indigo";
            document.documentElement.setAttribute("data-theme", next);
            try { localStorage.setItem("loom-theme", next); } catch (e) {}
            label();
          });
          label();
        }

        /* ---------- the draft glyph ----------
           Deterministic bit pattern from a seed string, so a story's
           draft is stable forever and unique to its slug.            */

        function draftBits(seed, count) {
          var h = 2166136261;
          for (var i = 0; i < seed.length; i++) {
            h ^= seed.charCodeAt(i);
            h = Math.imul(h, 16777619);
          }
          h = h >>> 0 || 1;
          var bits = [];
          for (var j = 0; j < count; j++) {
            h ^= h << 13; h >>>= 0;
            h ^= h >> 17;
            h ^= h << 5;  h >>>= 0;
            bits.push(h & 1);
          }
          return bits;
        }

        function draftEl(seed, count, extraClass) {
          var span = document.createElement("span");
          span.className = "draft" + (extraClass ? " " + extraClass : "");
          span.setAttribute("aria-hidden", "true");
          draftBits(seed, count).forEach(function (b) {
            var cell = document.createElement("i");
            if (b) cell.className = "set";
            span.appendChild(cell);
          });
          return span;
        }

        /* ---------- text ---------- */

        /* Collapses \r\n and stray \r — fixes \n\r\n showing up as
           a phantom blank line in generated credit blocks. */
        function normalizeText(s) {
          return String(s == null ? "" : s)
            .replace(/\r\n/g, "\n")
            .replace(/\r/g, "\n")
            .replace(/\n{3,}/g, "\n\n")
            .trim();
        }

        function escapeHtml(s) {
          return String(s)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;");
        }

        /* Small markdown subset: headings, hr, blockquote, bold, italic.
           Enough for generated prose, no dependency. */
        function renderProse(src) {
          var lines = normalizeText(String(src || "").replace(/\r\n?/g, "\n")).split("\n");
          var out = [], para = [], list = null, quote = [];

          function flushPara()  { if (para.length)  { out.push("<p>" + para.map(inline).join("<br>") + "</p>"); para = []; } }
          function flushQuote() { if (quote.length) { out.push("<blockquote>" + quote.map(inline).join("<br>") + "</blockquote>"); quote = []; } }
          function flushList()  {
            if (!list) return;
            out.push("<ul>" + list.map(function (li) { return "<li>" + inline(li) + "</li>"; }).join("") + "</ul>");
            list = null;
          }
          function flush() { flushPara(); flushList(); flushQuote(); }

          lines.forEach(function (raw) {
            var t = raw.trim(), m;
            if (!t) { flush(); return; }                                   // blank or space-only line
            if (/^(---|\*\*\*|___)$/.test(t)) { flush(); out.push("<hr>"); return; }
            if ((m = t.match(/^#{1,4}\s+(.*)$/))) { flush(); out.push("<h2>" + inline(m[1]) + "</h2>"); return; }
            if ((m = t.match(/^[-*+]\s+(.*)$/))) { flushPara(); flushQuote(); (list = list || []).push(m[1]); return; }
            if ((m = t.match(/^>\s?(.*)$/)))     { flushPara(); flushList(); quote.push(m[1]); return; }
            if (list && /^\s{2,}/.test(raw))     { list[list.length - 1] += " " + t; return; }  // indented continuation
            flushList(); flushQuote(); para.push(t);
          });
          flush();
          return out.join("\n");

          function inline(s) {
            return escapeHtml(s)
              .replace(/\*\*([^*]+)\*\*/g, "<strong>$1</strong>")
              .replace(/(^|[^*])\*([^*]+)\*/g, "$1<em>$2</em>")
              .replace(/(^|[^\w])_([^_]+)_(?=[^\w]|$)/g, "$1<em>$2</em>");
          }
        }

        function wordCount(s) {
          var t = normalizeText(s);
          return t ? t.split(/\s+/).length : 0;
        }

        function readMinutes(words) {
          return Math.max(1, Math.round(words / 230));
        }

        function formatDate(iso) {
          if (!iso) return "";
          var d = new Date(iso);
          if (isNaN(d)) return iso;
          return d.toLocaleDateString(undefined, {
            year: "numeric", month: "short", day: "numeric"
          });
        }

        /* ---------- data ---------- */

        function loadJSON(path) {
          return fetch(path, { cache: "no-cache" }).then(function (r) {
            if (!r.ok) throw new Error(r.status + " " + r.statusText + " — " + path);
            return r.json();
          });
        }      
      """;
      File.WriteAllText(filePath, jsContent);
      return filePath;
    }

    public static string WriteLoomCss(this FolderEntryClass folder, string outputFolder) {
      if (folder == null) {
        return "";
      }
      var filePath = Path.Combine(outputFolder, "loom.css");
      var cssContent = """
      
      :root {
        color-scheme: light dark;

        --ground:    #EDE7D9;
        --ground-2:  #E4DDCB;
        --ink:       #1A1F33;
        --ink-dim:   #5C6076;
        --ink-faint: #A49E8C;
        --madder:    #A3341F;
        --weld:      #7E600F;
        --cell-off:  #CFC7B2;

        --serif: "Iowan Old Style", "Palatino Linotype", "Book Antiqua",
                 Palatino, Charter, Georgia, serif;
        --mono: ui-monospace, "SF Mono", "Cascadia Mono", "Roboto Mono",
                Consolas, monospace;

        --measure: 66rem;
        --pad: clamp(1.25rem, 4vw, 3rem);
      }

      :root[data-theme="indigo"],
      :root[data-theme="auto"] {
        /* placeholder so the attribute selector always exists */
      }

      @media (prefers-color-scheme: dark) {
        :root:not([data-theme="linen"]) {
          --ground:    #131729;
          --ground-2:  #1B2038;
          --ink:       #E6E0D2;
          --ink-dim:   #93907F;
          --ink-faint: #565B72;
          --madder:    #D4573C;
          --weld:      #D2A63F;
          --cell-off:  #2A3050;
        }
      }

      :root[data-theme="indigo"] {
        --ground:    #131729;
        --ground-2:  #1B2038;
        --ink:       #E6E0D2;
        --ink-dim:   #93907F;
        --ink-faint: #565B72;
        --madder:    #D4573C;
        --weld:      #D2A63F;
        --cell-off:  #2A3050;
      }

      * { box-sizing: border-box; }

      html {
        -webkit-text-size-adjust: 100%;
        font-size: 19px;
      }

      body {
        margin: 0;
        background: var(--ground);
        color: var(--ink);
        font-family: var(--serif);
        font-size: 1rem;
        line-height: 1.65;
        text-rendering: optimizeLegibility;
        -webkit-font-smoothing: antialiased;
      }

      a { color: var(--madder); text-underline-offset: 2px; }
      a:focus-visible,
      button:focus-visible {
        outline: 2px solid var(--madder);
        outline-offset: 3px;
        border-radius: 1px;
      }

      /* ---------- machine layer: mono, tracked, small ---------- */

      .mono {
        font-family: var(--mono);
        font-size: 0.7rem;
        letter-spacing: 0.14em;
        text-transform: uppercase;
        color: var(--ink-dim);
      }

      .wrap {
        max-width: 52rem;
        margin: 0 auto;
        padding: 0 var(--pad);
      }

      /* ---------- masthead ---------- */

      .masthead {
        display: flex;
        align-items: baseline;
        justify-content: space-between;
        gap: 1rem;
        padding: 1.5rem 0 0;
      }

      .masthead a { text-decoration: none; color: var(--ink-dim); }
      .masthead a:hover { color: var(--madder); }

      .theme-toggle {
        font-family: var(--mono);
        font-size: 0.7rem;
        letter-spacing: 0.14em;
        text-transform: uppercase;
        background: none;
        border: 0;
        padding: 0.25rem 0;
        color: var(--ink-dim);
        cursor: pointer;
      }
      .theme-toggle:hover { color: var(--madder); }

      /* ---------- the draft glyph (signature element) ----------
         A weaving draft: filled and empty cells, generated from the
         story slug, so every story carries its own pattern.        */

      .draft {
        display: inline-grid;
        grid-auto-flow: column;
        gap: 2px;
        line-height: 0;
        vertical-align: middle;
      }

      .draft i {
        display: block;
        width: 6px;
        height: 6px;
        background: var(--cell-off);
      }

      .draft i.set { background: var(--ink-dim); }
      .draft i.lit { background: var(--madder); }

      .draft--lg i { width: 9px; height: 9px; }
      .draft--sm i { width: 4px; height: 4px; }

      /* ---------- index ---------- */

      .realm-head {
        padding: 3.5rem 0 0.5rem;
      }

      .realm-head h1 {
        font-size: clamp(2.5rem, 8vw, 4rem);
        font-weight: 400;
        letter-spacing: -0.02em;
        line-height: 1.05;
        margin: 0.4rem 0 0.6rem;
      }

      .realm-head p { margin: 0; }

      .filters {
        display: flex;
        flex-wrap: wrap;
        gap: 0.5rem;
        padding: 2rem 0 0;
      }

      .chip {
        font-family: var(--mono);
        font-size: 0.68rem;
        letter-spacing: 0.12em;
        text-transform: uppercase;
        background: none;
        border: 1px solid var(--cell-off);
        color: var(--ink-dim);
        padding: 0.35rem 0.7rem;
        cursor: pointer;
      }
      .chip:hover { border-color: var(--madder); color: var(--madder); }
      .chip[aria-pressed="true"] {
        border-color: var(--ink);
        color: var(--ground);
        background: var(--ink);
      }

      .shelf { padding: 2.5rem 0 5rem; margin: 0; }

      .entry {
        display: grid;
        grid-template-columns: auto 1fr;
        gap: 0 1.25rem;
        padding: 1.5rem 0;
        text-decoration: none;
        color: inherit;
      }

      .entry .draft { padding-top: 0.7rem; }
      .entry:hover .draft i.set { background: var(--madder); }

      .entry h2 {
        font-size: 1.6rem;
        font-weight: 400;
        letter-spacing: -0.01em;
        line-height: 1.2;
        margin: 0 0 0.35rem;
      }
      .entry:hover h2 { color: var(--madder); }

      .entry .meta { margin: 0 0 0.5rem; }

      .entry .blurb {
        margin: 0;
        color: var(--ink-dim);
        font-size: 0.95rem;
        line-height: 1.55;
        max-width: var(--measure);
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        overflow: hidden;
      }

      /* ---------- reader ---------- */

      .progress-bar {
        position: sticky;
        top: 0;
        z-index: 10;
        background: var(--ground);
        padding: 0.85rem 0;
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 1rem;
      }

      .progress-bar .draft { flex: 0 0 auto; }

      .story-head { padding: 3.5rem 0 2rem; }

      .story-head h1 {
        font-size: clamp(2.2rem, 7vw, 3.5rem);
        font-weight: 400;
        letter-spacing: -0.02em;
        line-height: 1.08;
        margin: 0.5rem 0 1rem;
      }

      .story-head .draft { margin-top: 0.5rem; }

      .prose {
        max-width: var(--measure);
        font-size: 1.15rem;
      }

      .prose p { margin: 0 0 1.35em; }

      .prose > p:first-of-type::first-line {
        font-variant-caps: small-caps;
        letter-spacing: 0.04em;
      }

      .prose h2 {
        font-family: var(--mono);
        font-size: 0.7rem;
        letter-spacing: 0.16em;
        text-transform: uppercase;
        color: var(--ink-dim);
        font-weight: 400;
        margin: 3rem 0 1rem;
      }

      .prose blockquote {
        margin: 1.5em 0;
        padding-left: 1.25em;
        border-left: 2px solid var(--cell-off);
        color: var(--ink-dim);
        font-style: italic;
      }

      .prose hr {
        border: 0;
        text-align: center;
        margin: 2.5em 0;
      }
      .prose hr::after {
        content: "· · ·";
        letter-spacing: 0.5em;
        color: var(--ink-faint);
      }
      .prose ul { padding-left: 1.25rem; margin: 0 0 1.25rem; }
      .prose li { margin: 0 0 0.5rem; }
      .prose li::marker { color: var(--ink-faint); }

      /* ---------- addendum ---------- */

      .addendum {
        max-width: var(--measure);
        margin: 4rem 0 0;
        padding: 2rem 0 0;
        border-top: 1px solid var(--cell-off);
      }

      .addendum .prose { font-size: 1rem; color: var(--ink-dim); }

      /* ---------- colophon ---------- */

      .colophon {
        margin: 4rem 0 6rem;
        padding: 2rem;
        background: var(--ground-2);
      }

      .colophon h2 {
        font-family: var(--mono);
        font-size: 0.7rem;
        letter-spacing: 0.16em;
        text-transform: uppercase;
        font-weight: 400;
        color: var(--weld);
        margin: 0 0 1.25rem;
      }

      .credit-grid {
        display: grid;
        grid-template-columns: max-content 1fr;
        gap: 0.5rem 1.5rem;
        font-family: var(--mono);
        font-size: 0.78rem;
        line-height: 1.5;
      }

      .credit-grid dt {
        color: var(--ink-dim);
        letter-spacing: 0.06em;
      }
      .credit-grid dd { margin: 0; color: var(--ink); }
      .credit-grid dd .host { color: var(--weld); }

      .credit-raw {
        font-family: var(--mono);
        font-size: 0.78rem;
        line-height: 1.6;
        white-space: pre-wrap;
        margin: 0;
        color: var(--ink);
      }

      /* ---------- realm page ---------- */

      .cover {
        display: block;
        width: 100%;
        max-height: 24rem;
        object-fit: cover;
        margin: 1.5rem 0 0;
      }

      .links {
        list-style: none;
        display: flex;
        flex-wrap: wrap;
        gap: 0.5rem 1.5rem;
        padding: 1.25rem 0 0;
        margin: 0;
      }
      .links a { text-decoration: none; }
      .links img { width: 14px; height: 14px; vertical-align: -2px; margin-right: 0.4rem; }

      .embed { max-width: var(--measure); margin: 1.5rem 0; }
      .embed iframe { width: 100%; border: 0; }

      #realm .prose { padding-top: 2rem; }

      .section-label {
        margin: 4rem 0 0;
        padding-top: 2rem;
        border-top: 1px solid var(--cell-off);
      }

      .entry .thumb {
        display: block;
        width: 5.5rem;
        height: 5.5rem;
        object-fit: cover;
      }

      /* ---------- album ---------- */

      .tracks {
        list-style: none;
        margin: 3rem 0 0;
        padding: 0;
        max-width: var(--measure);
      }

      .track {
        display: grid;
        grid-template-columns: auto 1fr;
        gap: 0 1.25rem;
        padding: 1.5rem 0;
        border-top: 1px solid var(--cell-off);
      }

      .track-no {
        font-family: var(--mono);
        font-size: 0.8rem;
        color: var(--ink-faint);
        padding-top: 0.45rem;
      }

      .track h3 {
        font-size: 1.35rem;
        font-weight: 400;
        line-height: 1.25;
        margin: 0 0 0.6rem;
      }

      .track .thumb {
        display: block;
        width: 8rem;
        height: 8rem;
        object-fit: cover;
        margin: 0 0 0.75rem;
      }

      .track audio,
      .track video { display: block; width: 100%; margin: 0.5rem 0; }

      .track .listen { text-decoration: none; }
      .track .listen:hover { color: var(--madder); }

      .track details { margin-top: 0.75rem; }
      .track summary { cursor: pointer; }
      .track summary:hover { color: var(--madder); }
      .track details .prose { font-size: 1rem; padding-top: 0.75rem; }

      .track .credit-raw { margin-top: 0.75rem; color: var(--ink-dim); }

      /* ---------- states ---------- */

      .note {
        padding: 4rem 0;
        max-width: var(--measure);
        color: var(--ink-dim);
      }
      .note strong { color: var(--ink); font-weight: 400; }
      .note code {
        font-family: var(--mono);
        font-size: 0.85em;
        color: var(--madder);
      }

      @media (max-width: 34rem) {
        body { font-size: 17px; }
        .entry { grid-template-columns: 1fr; gap: 0.5rem; }
        .entry .draft { padding-top: 0; }
        .colophon { padding: 1.5rem; }
        .credit-grid { grid-template-columns: 1fr; gap: 0.15rem 0; }
        .credit-grid dd { margin-bottom: 0.75rem; }
      }

      @media (prefers-reduced-motion: reduce) {
        * { transition: none !important; animation: none !important; }
      }

      @media print {
        .progress-bar, .masthead, .filters { display: none; }
        body { background: #fff; color: #000; }
      }
      
      """;
      File.WriteAllText(filePath, cssContent);
      return filePath;


    }

    public static string WriteRealmEntry(this RealmEntryClass realm, string outputFolder) {
      if (realm == null) {
        return "";
      }
      var filePath = Path.Combine(outputFolder, realm.File);
      var json = JsonSerializer.Serialize(realm, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(filePath, json);

      return filePath;
    }

    public static string WriteRealmHtml(this FolderEntryClass folder, string outputFolder) {
      if (folder == null) {
        return "";
      }
      var filePath = Path.Combine(outputFolder, "realm.html");
      var htmlContent = """
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>Realm</title>
        <link rel="stylesheet" href="loom.css">
    </head>
    <body>
    <div class="wrap">

        <header class="masthead">
            <a class="mono" href="index.html">&larr; all realms</a>
            <button class="theme-toggle" id="theme" type="button"></button>
        </header>

        <div class="realm-head">
            <p class="mono" id="eyebrow">Weavers Guild</p>
            <h1 id="title"></h1>
            <p class="mono" id="tally"></p>
            <div id="glyph"></div>
        </div>

        <main id="realm">
            <p class="note mono">Loading the realm&hellip;</p>
        </main>

    </div>
    <script src="loom.js"></script>
    <script>
    mountThemeToggle(document.getElementById("theme"));

    var file = new URLSearchParams(location.search).get("s") || "";
    var main = document.getElementById("realm");

    loadJSON("index.json").then(function (f) {
      if (f && f.name) document.getElementById("eyebrow").textContent = f.name;
    }).catch(function () {});

    if (!/^[A-Za-z0-9_.-]+\.json$/.test(file)) {
      main.innerHTML = '<div class="note"><strong>No realm selected.</strong><br>Open this page from the index.</div>';
    } else {
      loadJSON(file).then(render).catch(function (err) {
        main.innerHTML = '<div class="note"><strong>Could not load this realm.</strong><br>' +
          '<span class="mono">' + escapeHtml(err.message) + '</span></div>';
      });
    }

    function el(tag, cls, text) {
      var e = document.createElement(tag);
      if (cls) e.className = cls;
      if (text != null) e.textContent = text;
      return e;
    }

    function render(r) {
      document.title = r.name || "Realm";
      document.getElementById("title").textContent = r.name || "";
      document.getElementById("glyph").appendChild(draftEl(r.file || file, 16, "draft--lg"));

      var entries = r.entries || [];
      var albums = entries.filter(function (e) { return e.kind === "album"; }).length;
      var stories = entries.filter(function (e) { return e.kind === "story"; }).length;
      var tally = [];
      if (stories) tally.push(stories + (stories === 1 ? " story" : " stories"));
      if (albums) tally.push(albums + (albums === 1 ? " album" : " albums"));
      document.getElementById("tally").textContent = tally.join(" \u00B7 ");

      main.innerHTML = "";

      var cover = (r.images || []).filter(function (m) { return m.kind === "image"; })[0];
      if (cover) {
        var img = el("img", "cover");
        img.src = cover.src;
        img.alt = "";
        main.appendChild(img);
      }

      var links = r.links || [];
      var plain = links.filter(function (l) { return !l.embed && l.url; });
      if (plain.length) {
        var ul = el("ul", "links mono");
        plain.forEach(function (l) {
          var li = el("li");
          var a = el("a");
          a.href = l.url;
          a.rel = "noopener";
          if (l.iconUrl) { var ic = el("img"); ic.src = l.iconUrl; ic.alt = ""; a.appendChild(ic); }
          a.appendChild(document.createTextNode(l.name || l.url));
          li.appendChild(a);
          ul.appendChild(li);
        });
        main.appendChild(ul);
      }

      links.filter(function (l) { return l.embed; }).forEach(function (l) {
        var d = el("div", "embed");
        d.innerHTML = l.embed;   // own data; iframes render, inline scripts won't run
        main.appendChild(d);
      });

      if (r.content) {
        var prose = el("article", "prose");
        prose.innerHTML = renderProse(r.content);
        main.appendChild(prose);
      }

      if (!entries.length) return;

      main.appendChild(el("p", "mono section-label", "In this realm"));
      var shelf = el("div", "shelf");
      entries.forEach(function (e) {
        var a = el("a", "entry");
        a.href = (e.kind === "album" ? "album.html" : "story.html") + "?s=" + encodeURIComponent(e.file);
        if (e.cover) {
          var t = el("img", "thumb");
          t.src = e.cover;
          t.alt = "";
          a.appendChild(t);
        } else {
          a.appendChild(draftEl(e.file, 8));
        }
        var body = el("div");
        body.appendChild(el("h2", null, e.name));
        var bits = [e.kind];
        if (e.published) bits.push(formatDate(e.published));
        if (e.words) bits.push(readMinutes(e.words) + " min");
        body.appendChild(el("p", "mono meta", bits.join(" \u00B7 ")));
        if (e.blurb) body.appendChild(el("p", "blurb", e.blurb));
        a.appendChild(body);
        shelf.appendChild(a);
      });
      main.appendChild(shelf);
    }
    </script>
    </body>
    </html>
    """;
      File.WriteAllText(filePath, htmlContent);
      return filePath;
    }

    public static string WriteStoryHtml(this FolderEntryClass folder, string outputFolder) {
      if (folder == null) {
        return "";
      }
      var filePath = Path.Combine(outputFolder, "story.html");
      var htmlContent = """
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>The Loom</title>
        <link rel="stylesheet" href="loom.css">
    </head>
    <body>
    <div class="wrap">

        <div class="progress-bar">
            <a class="mono" id="back" href="index.html">&larr; all realms</a>
            <span id="progress"></span>
            <button class="theme-toggle" id="theme" type="button"></button>
        </div>

        <main id="main">
            <p class="note mono">Loading&hellip;</p>
        </main>

    </div>
    <script src="loom.js"></script>
    <script>
    mountThemeToggle(document.getElementById("theme"));

    var PROGRESS_CELLS = 24;
    var main = document.getElementById("main");
    var progressHost = document.getElementById("progress");
    var back = document.getElementById("back");

    var file = new URLSearchParams(location.search).get("s") || "";

    if (!/^[A-Za-z0-9_.-]+\.json$/.test(file)) {
      main.innerHTML = '<div class="note"><strong>No story named.</strong><br>' +
        'Open a story from its <a href="index.html">realm</a>.</div>';
    } else {
      loadJSON(file).then(renderStory).catch(function (err) {
        main.innerHTML = '<div class="note"><strong>Could not open that story.</strong><br>' +
          '<span class="mono">' + escapeHtml(err.message) + '</span></div>';
      });
    }

    function el(tag, cls, text) {
      var e = document.createElement(tag);
      if (cls) e.className = cls;
      if (text != null) e.textContent = text;
      return e;
    }

    /* The header already shows the name; drop a leading single-# title line. */
    function stripTitle(md) {
      return String(md || "").replace(/^\s*#(?!#)[^\n]*\n?/, "");
    }

    function renderStory(s) {
      document.title = s.name + " \u00B7 " + (s.realmName || "Threadspace");

      if (s.realmFile) {
        back.href = "realm.html?s=" + encodeURIComponent(s.realmFile);
        back.textContent = "\u2190 " + (s.realmName || "realm");
      }

      var content = stripTitle(s.content);
      var words = s.words || wordCount(content);

      var meta = [];
      if (s.realmName) meta.push(s.realmName);
      if (s.published) meta.push(formatDate(s.published));
      if (words) meta.push(readMinutes(words) + " min");

      var head = el("header", "story-head");
      head.appendChild(el("p", "mono", meta.join(" \u00B7 ")));
      head.appendChild(el("h1", null, s.name));
      head.appendChild(draftEl(s.slug || file, 16, "draft--lg"));
      if (s.cover) {
        var img = el("img", "cover");
        img.src = s.cover;
        img.alt = "";
        head.appendChild(img);
      }

      main.innerHTML = "";
      main.appendChild(head);

      var article = el("article", "prose");
      article.innerHTML = renderProse(content);
      main.appendChild(article);

      var links = s.links || [];
      links.filter(function (l) { return l.embed; }).forEach(function (l) {
        var d = el("div", "embed");
        d.innerHTML = l.embed;
        main.appendChild(d);
      });
      var plain = links.filter(function (l) { return !l.embed && l.url; });
      if (plain.length) {
        var ul = el("ul", "links mono");
        plain.forEach(function (l) {
          var li = el("li");
          var a = el("a");
          a.href = l.url;
          a.rel = "noopener";
          if (l.iconUrl) { var ic = el("img"); ic.src = l.iconUrl; ic.alt = ""; a.appendChild(ic); }
          a.appendChild(document.createTextNode(l.name || l.url));
          li.appendChild(a);
          ul.appendChild(li);
        });
        main.appendChild(ul);
      }

      if (s.realmNote) {
        var add = el("section", "addendum");
        add.appendChild(el("p", "mono", "Addendum \u00B7 " + (s.realmName || "Realm")));
        var ap = el("div", "prose");
        ap.innerHTML = renderProse(s.realmNote);
        add.appendChild(ap);
        main.appendChild(add);
      }

      main.appendChild(colophon(s, words));

      progressHost.appendChild(draftEl(s.slug || file, PROGRESS_CELLS));
      trackProgress();
    }

    function colophon(s, words) {
      var sec = el("section", "colophon");
      sec.appendChild(el("h2", null, "Colophon"));

      var credits = s.credits;
      if (Array.isArray(credits) && credits.length) {
        var dl = el("dl", "credit-grid");
        credits.forEach(function (c) {
          dl.appendChild(el("dt", null, c.role || "Contributor"));
          var dd = el("dd", null, c.model || c.name || "");
          if (c.host) dd.appendChild(el("span", "host", " @ " + c.host));
          dl.appendChild(dd);
        });
        sec.appendChild(dl);
      } else if (typeof credits === "string" && credits.trim()) {
        sec.appendChild(el("p", "credit-raw", normalizeText(credits)));
      }

      var foot = el("dl", "credit-grid");
      foot.style.marginTop = credits ? "1.25rem" : "0";
      if (s.published) {
        foot.appendChild(el("dt", null, "Published"));
        foot.appendChild(el("dd", null, formatDate(s.published)));
      }
      if (words) {
        foot.appendChild(el("dt", null, "Length"));
        foot.appendChild(el("dd", null, words.toLocaleString() + " words"));
      }
      if (foot.children.length) sec.appendChild(foot);
      return sec;
    }

    /* The draft band in the bar doubles as reading progress. */
    function trackProgress() {
      var cells = progressHost.querySelectorAll("i");
      if (!cells.length) return;
      var tick = function () {
        var h = document.documentElement;
        var span = h.scrollHeight - h.clientHeight;
        var pct = span > 0 ? h.scrollTop / span : 1;
        var lit = Math.round(pct * cells.length);
        for (var i = 0; i < cells.length; i++) cells[i].classList.toggle("lit", i < lit);
      };
      var queued = false;
      addEventListener("scroll", function () {
        if (queued) return;
        queued = true;
        requestAnimationFrame(function () { queued = false; tick(); });
      }, { passive: true });
      tick();
    }
    </script>
    </body>
    </html>
    """;
      File.WriteAllText(filePath, htmlContent);
      return filePath;
    }

    public static string WriteAlbumHtml(this FolderEntryClass folder, string outputFolder) {
      if (folder == null) {
        return "";
      }
      var filePath = Path.Combine(outputFolder, "album.html");
      var htmlContent = """
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>The Loom</title>
        <link rel="stylesheet" href="loom.css">
    </head>
    <body>
    <div class="wrap">

        <header class="masthead">
            <a class="mono" id="back" href="index.html">&larr; all realms</a>
            <button class="theme-toggle" id="theme" type="button"></button>
        </header>

        <main id="main">
            <p class="note mono">Loading&hellip;</p>
        </main>

    </div>
    <script src="loom.js"></script>
    <script>
    mountThemeToggle(document.getElementById("theme"));

    var main = document.getElementById("main");
    var back = document.getElementById("back");
    var file = new URLSearchParams(location.search).get("s") || "";

    if (!/^[A-Za-z0-9_.-]+\.json$/.test(file)) {
      main.innerHTML = '<div class="note"><strong>No album named.</strong><br>' +
        'Open an album from its <a href="index.html">realm</a>.</div>';
    } else {
      loadJSON(file).then(renderAlbum).catch(function (err) {
        main.innerHTML = '<div class="note"><strong>Could not open that album.</strong><br>' +
          '<span class="mono">' + escapeHtml(err.message) + '</span></div>';
      });
    }

    function el(tag, cls, text) {
      var e = document.createElement(tag);
      if (cls) e.className = cls;
      if (text != null) e.textContent = text;
      return e;
    }

    /* PublishedUrl can be a page (SoundCloud) rather than a file. Only real files get a player. */
    function playable(src, kind) {
      var re = kind === "video"
        ? /\.(mp4|webm|mov)(\?|#|$)/i
        : /\.(mp3|wav|flac|ogg|m4a|aac|opus)(\?|#|$)/i;
      return re.test(src || "");
    }

    function appendLinks(host, links) {
      (links || []).filter(function (l) { return l.embed; }).forEach(function (l) {
        var d = el("div", "embed");
        d.innerHTML = l.embed;
        host.appendChild(d);
      });
      var plain = (links || []).filter(function (l) { return !l.embed && l.url; });
      if (!plain.length) return;
      var ul = el("ul", "links mono");
      plain.forEach(function (l) {
        var li = el("li");
        var a = el("a");
        a.href = l.url;
        a.rel = "noopener";
        if (l.iconUrl) { var ic = el("img"); ic.src = l.iconUrl; ic.alt = ""; a.appendChild(ic); }
        a.appendChild(document.createTextNode(l.name || l.url));
        li.appendChild(a);
        ul.appendChild(li);
      });
      host.appendChild(ul);
    }

    function appendCredits(host, credits) {
      if (typeof credits === "string" && credits.trim()) {
        host.appendChild(el("p", "credit-raw", normalizeText(credits)));
      }
    }

    function renderAlbum(a) {
      document.title = a.name + " \u00B7 " + (a.realmName || "Threadspace");

      if (a.realmFile) {
        back.href = "realm.html?s=" + encodeURIComponent(a.realmFile);
        back.textContent = "\u2190 " + (a.realmName || "realm");
      }

      var tracks = a.tracks || [];
      var meta = ["album"];
      if (a.realmName) meta.push(a.realmName);
      if (a.published) meta.push(formatDate(a.published));
      if (tracks.length) meta.push(tracks.length + (tracks.length === 1 ? " track" : " tracks"));

      var head = el("header", "story-head");
      head.appendChild(el("p", "mono", meta.join(" \u00B7 ")));
      head.appendChild(el("h1", null, a.name));
      head.appendChild(draftEl(a.slug || file, 16, "draft--lg"));
      if (a.cover) {
        var img = el("img", "cover");
        img.src = a.cover;
        img.alt = "";
        head.appendChild(img);
      }

      main.innerHTML = "";
      main.appendChild(head);

      appendLinks(main, a.links);

      if (a.content) {
        var notes = el("article", "prose");
        notes.innerHTML = renderProse(a.content);
        main.appendChild(notes);
      }

      if (tracks.length) {
        var ol = el("ol", "tracks");
        tracks.forEach(function (t, i) { ol.appendChild(renderTrack(t, i, a.cover)); });
        main.appendChild(ol);
      }

      if (a.credits && a.credits.trim()) {
        var col = el("section", "colophon");
        col.appendChild(el("h2", null, "Colophon"));
        appendCredits(col, a.credits);
        main.appendChild(col);
      }

      /* one track at a time */
      main.addEventListener("play", function (e) {
        main.querySelectorAll("audio, video").forEach(function (m) { if (m !== e.target) m.pause(); });
      }, true);
    }

    function renderTrack(t, i, albumCover) {
      var li = el("li", "track");
      li.appendChild(el("span", "track-no", String(i + 1).padStart(2, "0")));

      var body = el("div");
      body.appendChild(el("h3", null, t.name));

      var media = t.media || [];
      var art = media.filter(function (m) { return m.kind === "image" && m.src !== albumCover; })[0];
      if (art) {
        var img = el("img", "thumb");
        img.src = art.src;
        img.alt = "";
        body.appendChild(img);
      }

      var av = media.filter(function (m) { return m.kind === "audio" || m.kind === "video"; })[0];
      if (av) {
        if (playable(av.src, av.kind)) {
          var player = el(av.kind === "video" ? "video" : "audio");
          player.controls = true;
          player.preload = "none";
          player.src = av.src;
          body.appendChild(player);
        } else if (av.src) {
          var listen = el("a", "mono listen", "listen \u2192");
          listen.href = av.src;
          listen.rel = "noopener";
          body.appendChild(listen);
        }
      }

      appendLinks(body, t.links);

      if (t.content && t.content.trim()) {
        var d = el("details");
        d.appendChild(el("summary", "mono", "lyrics"));
        var p = el("div", "prose");
        p.innerHTML = renderProse(t.content);
        d.appendChild(p);
        body.appendChild(d);
      }

      appendCredits(body, t.credits);
      li.appendChild(body);
      return li;
    }
    </script>
    </body>
    </html>
    """;
      File.WriteAllText(filePath, htmlContent);
      return filePath;
    }

  }
}

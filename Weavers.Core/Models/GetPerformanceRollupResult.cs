using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Models {
  public class GetPerformanceRollupResult {
    public RealmDto Realm { get; set; } = new RealmDto();
    public StoryDto Story { get; set; } = new StoryDto();
    public SceneDto Scene { get; set; } = new SceneDto();
    public List<CharacterDto> Characters { get; set; } = new List<CharacterDto>();
    public PerformanceDto Performance { get; set; } = new PerformanceDto();
  }

  public class RealmDto {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tone { get; set; } = string.Empty;
  }

  public class StoryDto {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Card { get; set; } = string.Empty;
    public int TargetSceneCount { get; set; }
  }

  public class SceneDto {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Rank { get; set; }
    public string EntryState { get; set; } = string.Empty;
    public string ExitState { get; set; } = string.Empty;
    public string Pov { get; set; } = string.Empty; // resolved label, e.g. "Third-person limited"
  }

  public class CharacterDto {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? VoiceNote { get; set; } = null;
  }

  public class PerformanceDto {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool ActorPerformed { get; set; } = false;
    public List<EntryDto> Entries { get; set; } = new List<EntryDto>();
  }

  public class EntryDto {
    public int Rank { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? CharacterId { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
  }
}

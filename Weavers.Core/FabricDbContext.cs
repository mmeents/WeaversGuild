
using KB.Core;
using Microsoft.EntityFrameworkCore;  
using KB.Core.Entities;

namespace Weavers.Core {
  public class FabricDbContext : KbDbContext {
    public FabricDbContext(DbContextOptions<FabricDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(FabricDbContext).Assembly);

      // ====================== FABRIC ITEM TYPES ======================
      modelBuilder.Entity<ItemType>().HasData(
          new ItemType { Id = 100, Name = "Idea", Description = "Raw product ideas or chicken-and-egg problems (starting point)" },
          new ItemType { Id = 101, Name = "Capability", Description = "What the stack, agents, or developer can currently do reliably" },
          new ItemType { Id = 102, Name = "Constraint", Description = "Scope, tech, time, or client-type limitations" },
          new ItemType { Id = 103, Name = "Requirement", Description = "Functional or non-functional specifications / user stories" },
          new ItemType { Id = 104, Name = "ResearchFinding", Description = "Market, competitor, feasibility, or validation insights" },
          new ItemType { Id = 105, Name = "DomainEntity", Description = "Business domain object in the app (e.g. Task, Record)" },
          new ItemType { Id = 106, Name = "Command", Description = "MediatR command (e.g. CreateTaskCommand)" },
          new ItemType { Id = 107, Name = "Query", Description = "MediatR query" },
          new ItemType { Id = 108, Name = "Handler", Description = "MediatR command/query handler" },
          new ItemType { Id = 109, Name = "ClientApp", Description = "WinFormsClient, AngularClient, ConsoleTester, etc." },
          new ItemType { Id = 110, Name = "Agent", Description = "AI agent role (ResearchAgent, LayerBuilderAgent, etc.)" },
          new ItemType { Id = 111, Name = "Layer", Description = "Build layer in the agent chain (RequirementsLayer, InfraLayer, etc.)" },
          new ItemType { Id = 112, Name = "Feature", Description = "Scoped feature or user story" },
          new ItemType { Id = 113, Name = "Decision", Description = "Architectural or scoping decision with rationale" },
          new ItemType { Id = 114, Name = "LessonLearned", Description = "Post-iteration insights and capability updates" },
          new ItemType { Id = 115, Name = "Artifact", Description = "Generated output (code snippet, diagram, mock, document)" },
          new ItemType { Id = 116, Name = "PoC", Description = "Proof-of-concept experiment or vertical slice" }
      );

      // ====================== FABRIC RELATION TYPES ======================
      modelBuilder.Entity<ItemRelationType>().HasData(
          // Research & Planning
          new ItemRelationType { Id = 200, Relation = "INSPIRES", Description = "Idea → Idea / Idea → Requirement" },
          new ItemRelationType { Id = 201, Relation = "ADDRESSES", Description = "Requirement → Idea" },
          new ItemRelationType { Id = 202, Relation = "REFINES", Description = "ResearchFinding → Idea / Requirement" },
          new ItemRelationType { Id = 203, Relation = "CONSTRAINS", Description = "Constraint → Idea / Requirement / Capability" },
          new ItemRelationType { Id = 204, Relation = "VALIDATES", Description = "ResearchFinding / PoC → Requirement / Capability" },
          new ItemRelationType { Id = 205, Relation = "RESOLVES", Description = "Decision → Constraint" },

          // Capability & Scope
          new ItemRelationType { Id = 206, Relation = "ENABLES", Description = "Capability → Requirement / Feature" },
          new ItemRelationType { Id = 207, Relation = "LIMITS", Description = "Constraint → Capability" },
          new ItemRelationType { Id = 208, Relation = "DEPENDS_ON", Description = "Requirement / Feature → Capability" },
          new ItemRelationType { Id = 209, Relation = "EXTENDS", Description = "Capability → Capability (stack evolution)" },

          // Layered Agent Chain (the heart of the WeaversGuild Fabric)
          new ItemRelationType { Id = 210, Relation = "BUILDS_ON", Description = "Previous Layer / Artifact → Next Layer" },
          new ItemRelationType { Id = 211, Relation = "PRODUCES", Description = "Agent → Artifact / Layer / Requirement" },
          new ItemRelationType { Id = 212, Relation = "CONSUMES", Description = "Agent → ResearchFinding / Capability" },
          new ItemRelationType { Id = 213, Relation = "ASSIGNS_TO", Description = "Orchestrator / Layer → Agent" },
          new ItemRelationType { Id = 214, Relation = "EXECUTES", Description = "Agent → Command / Handler" },
          new ItemRelationType { Id = 215, Relation = "IMPLEMENTS", Description = "Handler → Command / Query" },
          new ItemRelationType { Id = 216, Relation = "EXPOSES_TO", Description = "Core / MediatR → ClientApp" },

          // MediatR & Domain Specific
          new ItemRelationType { Id = 217, Relation = "HANDLES", Description = "Handler → Command / Query" },
          new ItemRelationType { Id = 218, Relation = "TRIGGERS", Description = "Command → Handler" },
          new ItemRelationType { Id = 219, Relation = "RETURNS", Description = "Handler → DomainEntity" },
          new ItemRelationType { Id = 220, Relation = "USES", Description = "ClientApp → Command / Query" },
          new ItemRelationType { Id = 221, Relation = "BELONGS_TO", Description = "DomainEntity → DomainEntity (relationships)" },
          new ItemRelationType { Id = 222, Relation = "REQUIRES", Description = "Feature / Requirement → DomainEntity / Handler" },

          // Feedback & Iteration
          new ItemRelationType { Id = 223, Relation = "INFORMS", Description = "LessonLearned → Capability / Decision" },
          new ItemRelationType { Id = 224, Relation = "RESULTS_IN", Description = "PoC / Experiment → LessonLearned / Requirement" },

          // General Utility
          new ItemRelationType { Id = 225, Relation = "CONTAINS", Description = "Container → Item (Layer → Feature, etc.)" },
          new ItemRelationType { Id = 226, Relation = "DERIVES_FROM", Description = "Artifact → Requirement / Idea" },
          new ItemRelationType { Id = 227, Relation = "REFERENCES", Description = "Loose reference between any two items" },
          new ItemRelationType { Id = 228, Relation = "PRECEDES", Description = "Sequencing: Layer A precedes Layer B" },
          new ItemRelationType { Id = 229, Relation = "FOLLOWS", Description = "Sequencing: Layer B follows Layer A" }
      );
    }
  }
}

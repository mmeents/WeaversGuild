# Input
You are provided with the content of an RSS item. It may contain HTML tags, navigation menus, and footers.

# Instructions
1. **Ignore Noise:** Completely disregard HTML tags, navigation links, footers, comments, and advertisements.
2. **Identify Core Topic:** What is the main technical concept, tutorial, or news item?
3. **Extract Key Points:** List 3-5 bullet points summarizing the most valuable technical insights.
4. **Identify Tags:** Extract relevant technical tags (e.g., #Azure, #CSharp, #Blazor).
5. **Actionable Takeaway:** What should a developer do with this information? (e.g., "Read this if you are migrating to OIDC," "Use this pattern for EF Core.")

# Output Format
Create a Markdown block with the following structure:

---
## 📚 Knowledge Card: [Title]
**Source:** [RSS Item Name]
**Date:** [Date]
**Tags:** #Tag1 #Tag2 #Tag3

### 💡 Summary
[A 2-sentence summary of the core value.]

### 🔑 Key Insights
- [Point 1]
- [Point 2]
- [Point 3]

### 🚀 Action Item
[What should the team do next? Or what problem does this solve?]

---

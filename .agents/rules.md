# AI System Architect & Governance Prompt

**MANDATORY INSTRUCTION FOR ALL AI AGENTS:**
Whenever you are initialized in this repository or starting a new project based on this template, you MUST prioritize the documentation-driven task management hierarchy. You are acting as the AI System Architect. **This file (`.agents/rules.md`) MUST be read and applied before any action is taken in every single chat interaction.**

## 1. Establish & Maintain the Governance Structure
Ensure the `governance/` folder exists and is actively used in the root directory. This directory is the single source of truth for the project:
- `governance/docs/architecture/standards.md`: Central guidelines (Clean Code, Security, Zero-Warning Policy).
- `governance/docs/architecture/system-glossary.md`: **READ THIS FIRST.** The complete map of the codebase.
- `governance/docs/architecture/decision-log.md`: The "Why" behind architectural choices.
- `governance/sprints/index.md`: The master index tracking all project sprints.

## 2. Rules of Engagement (The AI Workflow)
Before executing ANY task or writing ANY application code, you must strictly follow this sequence:
1. **Understand Context WITHOUT Grepping:** Read `governance/docs/architecture/system-glossary.md` to understand where things are. Do NOT scan the entire codebase blindly.
2. **Review Past Decisions:** Check `governance/docs/architecture/decision-log.md` to align with the current architecture.
3. **Read the Standards:** Internalize `governance/docs/architecture/standards.md`. You must enforce a strict 0 SonarQube warnings / 0 bugs policy.
4. **Plan Documentally:** Check the current sprint in `governance/sprints/index.md`. Create or update the corresponding task markdown files in the `tasks/` folder. 

## 3. Advanced AI Behaviors (Enterprise Standards)
- **TDD (Test-Driven Development):** You must write unit tests for every new feature or module. No code is complete without tests.
- **Boy Scout Rule:** While modifying a file, clean up any existing code smells or linter errors you encounter. Leave the code better than you found it.
- **Dependency Approval:** Do NOT add new third-party libraries without asking the user for explicit permission first.
- **Observability:** Do not use raw `console.log`. Always utilize the project's centralized logging mechanism.
- **Git Standards:** Use Conventional Commits (`feat:`, `fix:`, `chore:`) and work on isolated feature branches for each task.

## 4. Active Maintenance (Keep the Map Alive)
- **Update the Glossary:** If you create a new module, database model, or file structure, you MUST add it to `system-glossary.md`.
- **Log Decisions:** If you introduce a new library or structural pattern, add an entry to `decision-log.md`.
- **Sprint Tracking:** Actively check off completed tasks (`[x]`) and log new sub-tasks. When a sprint is finished, create the next sprint directory.
- **No Orphaned Docs:** If you create a new documentation file (e.g., in `governance/docs/examples`), you MUST add a link to it in that directory's `index.md` file. Never leave a file without a corresponding index entry.

## 5. Strict Compliance
You are forbidden from writing undocumented, unstructured code. Do not bypass the `governance` structure. Every architectural decision, feature addition, or refactor must first be planned as a task in the active sprint and mapped in the glossary.

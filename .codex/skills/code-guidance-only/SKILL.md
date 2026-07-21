---
name: code-guidance-only
description: Review and guide development in this C# WinForms database-browser project without writing or editing its business code. Use for architecture, models, services, database providers, WinForms flows, debugging guidance, code reviews, learning feedback, implementation plans, and pseudocode in this project.
---

# Code Guidance Only

Preserve the learning boundary: let the user implement the application code.

## Required behavior

1. Inspect relevant project files before assessing them.
2. Explain what is already correct before identifying gaps.
3. Separate correctness problems, design improvements, naming/style issues, and optional future enhancements.
4. Explain why each issue matters at the user's current beginner level.
5. Give a small, ordered next-step checklist.
6. Use field lists, method signatures in plain text, flow descriptions, or pseudocode when an example is useful.
7. Ask the user to implement the changes, then offer to review the result.

## Boundary

- Do not create, edit, or replace application source files such as `.cs`, designer files, project files, tests, SQL implementation scripts, or configuration containing application logic.
- Do not provide a complete copy-paste-ready implementation of a class, method, form, or service.
- Do not silently fix issues found during review.
- Do not design the WinForms layout unless the user asks for design guidance; the user owns the interface design.
- Editing documentation and this Skill is allowed when explicitly requested.
- Read-only inspection, builds, and tests are allowed when useful, but explain findings rather than repairing failures.
- If the user explicitly changes this boundary later, follow the newest explicit instruction.

## Pseudocode style

Keep pseudocode language-neutral and incomplete enough to require the user to make implementation decisions.

```text
LoadTables(connection, scope):
    validate input
    select provider for database type
    request table metadata
    map provider result into TableInfo objects
    return ordered list
```

Avoid C# bodies with exact namespaces, constructors, package APIs, and full error handling that could be pasted directly into the project.

## Review format

Use this order when reviewing code:

1. Current completion level
2. What is done well
3. Problems that should be fixed now
4. Improvements that can wait
5. Suggested model shape or pseudocode
6. The next implementation exercise

Prioritize a small viable model over speculative fields. For multi-database abstractions, distinguish shared concepts from Oracle-, SQL Server-, and MySQL-specific metadata.

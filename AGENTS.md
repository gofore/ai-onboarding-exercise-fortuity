# Contribution Instructions

## Commit Messages

- Write commit messages in English.
- Use Conventional Commits with a mandatory scope: `type(scope): description`.
- Allowed types are `feat`, `fix`, `docs`, `test`, `refactor`, `perf`, `build`, `ci`, and `chore`.
- Use a scope that names the part of the codebase affected by the change.
- Example: `docs(readme): add architecture diagram`.

## Pull Request Descriptions

- Write pull request descriptions in English.
- Include these headers in this exact order:

  1. `Background`: where the task came from, such as a ticket, exercise, or observed problem.
  2. `What was done`: summarize the changes in a few sentences; do not provide a file list.
  3. `How it was verified`: state what was run and observed, such as tests, startup, or curl checks.
  4. `Notes for the reviewer`: list open questions, assumptions, and anything deliberately left out.
  5. `AI assistance`: identify which parts the agent produced and what the author checked personally.
# Contributing to Hex Grid Package

Thank you for your interest in contributing! We welcome bug reports, feature requests, documentation improvements, and code contributions.

## 1. How to Submit an Issue

* Before creating a new issue, search existing issues to avoid duplicates.
* Provide a clear and descriptive title.
* Include the following information in the issue body:

  * **Unity version** and **platform** (e.g., Unity 2024.2.0f1, Windows 10)
  * **Steps to reproduce** the problem or behavior.
  * **Expected vs. actual** behavior.
  * Any relevant **console logs** or **screenshots**.

## 2. How to Propose a Change (Pull Request)

1. **Fork** the repository and create a new branch from `main`:

   ```bash
   git checkout -b feature/your-feature-name
   ```
2. **Implement** your changes, following the existing code style:

   * **C# coding conventions**: PascalCase for types and methods, camelCase for variables.
   * **XML documentation** for public APIs.
3. **Test** your changes:

   * Verify that the demo scene still works.
   * Run any existing tests (if applicable).
4. **Commit** your changes with a descriptive message:

   ```bash
   git commit -m "Add <feature>: brief description"
   ```
5. **Push** your branch and open a Pull Request against `main`.

## 3. Branching & Versioning

* **`main`** branch is always stable and reflects the latest published version.
* Create feature branches named `feature/xyz` or `bugfix/abc`.
* Include **CHANGELOG** updates for any user-visible changes.

## 4. Code Style & Guidelines

* **Indentation**: 4 spaces, no tabs.
* **Line length**: Keep lines under 120 characters.
* **Comments**: Use XML comments for public members.
* **Folders**: Place runtime scripts under `Runtime/`, editor scripts under `Editor/`, and samples under `Samples~/`.

## 5. Pull Request Checklist

* [ ] My changes follow the code style of this project.
* [ ] I have added/updated necessary documentation (README, doc-comments).
* [ ] I have tested my changes in Unity Editor.
* [ ] I have updated the CHANGELOG if applicable.

## 6. License & Code of Conduct

By contributing, you agree that your contributions will be licensed under the MIT License. Please see `LICENSE.txt`.

Please adhere to our [Code of Conduct](CODE_OF_CONDUCT.md) in all interactions.

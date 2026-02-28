# Contributing to This Project

First off, thank you for considering contributing to this project! 

This document outlines the end-to-end workflow, from setting up the project on your local machine to submitting a Pull Request (PR), alongside our version control (Git) workflow and coding standards. Adhering to these rules is mandatory to ensure a maintainable, clean, and bug-free codebase.

## 1. Getting Started: Project Setup

Follow these steps to get the project running on your local environment.

### Prerequisites
Make sure you have the following installed:
* [Git](https://git-scm.com/)
* [Docker](https://www.docker.com/) & [Docker Compose](https://docs.docker.com/compose/)

### Step 1: Clone the Repository
First, clone the repository to your local machine and navigate into the project directory:
```bash
git clone <repository-url>
cd <project-directory>
```

### Step 2: Environment Variables
We use environment variables to manage sensitive configurations. 
1. Locate the `.env.example` file in the root directory.
2. Duplicate it and rename the copy to `.env`.
3. Fill in the required values (database credentials, API keys, etc.) in the `.env` file. *Never commit your `.env` file to version control.*

### Step 3: Run the Application
We use Docker to ensure consistent development environments. To build and start the application and its dependencies:
```bash
docker-compose up -d --build
```
*The application should now be running. Check the specific service ports in the `docker-compose.yml` file.*

## 2. Branching Strategy

Our project utilizes a structured Git Flow model.

### Main Branches
* **`main`:** This branch contains only stable, thoroughly tested code that is running in the production environment. Direct commits to this branch are strictly prohibited; all changes must be merged via Pull Requests.
* **`develop`:** This is the primary branch for the development environment. All new features (`feature`) and bug fixes are initially merged and tested here.

### Supporting Branches
These branches are created temporarily for specific tasks and must be deleted after being merged into the relevant main branch.

* **`feature/<feature-name>`:** Created from the **`develop`** branch when working on a new feature or module. Once the work is complete, it is merged back into `develop`.
  * *Example:* `feature/user-authentication`
* **`bugfix/<bug-name>`:** Created from **`develop`** to fix bugs discovered during the development phase (before reaching production).
  * *Example:* `bugfix/login-crash-issue`
* **`refactor/<refactor-name>`:** Created from the **`develop`** branch when restructuring existing code without changing its external behavior or adding new features.
  * *Example:* `refactor/user-service-logic`
* **`hotfix/<bug-name>`:** Created directly from the **`main`** branch to address critical, urgent issues in the production environment. Once resolved, the fix must be merged into **both** `main` and `develop` to ensure the bug does not reappear in future releases.
  * *Example:* `hotfix/payment-gateway-timeout`

## 3. Development Workflow

Once your environment is running, follow these steps to start coding.

### Step 1: Update Your Local `develop` Branch
Always start your work from the most up-to-date `develop` branch:
```bash
git checkout develop
git pull origin develop
```

### Step 2: Create a New Branch
Create a supporting branch for your task based on the strategy outlined above:
* **`feature/...`** for new additions.
* **`bugfix/...`** for fixing issues.
* **`refactor/...`** for code restructuring.
* **`hotfix/...`** for urgent production patches.

```bash
git checkout -b feature/your-feature-name
```

### Step 3: Code and Test
Write your code! Ensure that your changes do not break existing functionality. Run any local test suites and linting tools before committing.

## 4. Commit Message Standards

We follow "Conventional Commits" standards to maintain a clean, readable commit history that is compatible with automated versioning (semver) tools.

### Format
`<type>(<scope>): <short_description>`

### Types
* **`feat:`** Used when adding a new feature.
* **`fix:`** Used when fixing a bug in the system.
* **`hotfix:`** Used for urgent patches in the production environment.
* **`docs:`** Used for changes strictly related to documentation (e.g., `README`, `CONTRIBUTING.md`, inline comments).
* **`chore:`** Used for maintenance tasks, dependency updates, or build process adjustments that do not affect production code.
* **`refactor:`** Used for code changes that neither fix a bug nor add a feature, but improve code structure and readability.
* **`test:`** Used when adding missing tests or correcting existing ones.

### Commit Examples
* `feat(auth): add Google login integration`
* `fix(cart): resolve incorrect total calculation`
* `hotfix(db): patch urgent database connection drop`
* `docs(readme): add git flow and commit standards`
* `chore(deps): bump axios version for security patch`
* `refactor(api): restructure user controller for better readability`

## 5. Submitting Your Work

### Step 1: Push Your Branch
Push your newly created branch to the remote repository:
```bash
git push origin feature/your-feature-name
```

### Step 2: Open a Pull Request (PR)
1. Navigate to the repository on GitHub/GitLab.
2. Open a Pull Request targeting the **`develop`** branch (or `main` if it is a `hotfix`).
3. Provide a clear title and description of your changes. 
4. Wait for code review. Make any requested changes by pushing new commits to your branch.

Once approved, a maintainer will merge your PR. Thank you for your contribution!
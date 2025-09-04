# Quickdraw

Quickdraw is a first-person shooter survival game in which the player must fend off against hoards of enemies until they are faced with the final boss.
Upon defeating the boss the player will win the game.
This game is a 4 person Unity project for a university course on graphics and modelling.


# Unity GIT checklist:

This project uses Unity 6.2 and Git for version control.
Follow these steps to set up and contribute correctly.

✅ Setup (One-Time)

Install tools

Unity Hub + the same Unity version (see ProjectSettings/ProjectVersion.txt)

Git (https://git-scm.com/downloads
)

Git LFS (https://git-lfs.com
) → install and enable:

git lfs install


Clone the repo

git clone repo-url
cd project-folder
git lfs pull


Open in Unity Hub

Add the folder as a project.

Unity will automatically download required packages from manifest.json.

📦 What’s in Git (must be committed)

Assets/ → All scripts, scenes, prefabs, materials, custom plugins

Packages/manifest.json → Lists project dependencies (Unity packages)

Packages/packages-lock.json → Locks exact versions of dependencies

ProjectSettings/ → Project settings, input mappings, tags/layers, build settings

.gitignore & .gitattributes → Ensure clean versioning across OS

🚫 What’s NOT in Git

Library/, Temp/, Obj/, Build/, Logs/ (Unity regenerates these)

Unity package cache (Unity will download automatically)

Local IDE settings (.vs/, .idea/, .vscode/)

🔄 Daily Workflow

Update project

git checkout main
git pull origin main


Create a feature branch

git checkout -b feature/my-task


Work in Unity / scripts
Save & test changes.

Stage & commit

git add .
git commit -m "Describe your change"


Push branch

git push -u origin feature/my-task


Open a Pull Request
Merge into main after review.

🧩 Packages (Important!)

Unity will auto-install dependencies from manifest.json.

If you add a new package (e.g., NavMesh, Cinemachine, etc.), always commit:

Packages/manifest.json

Packages/packages-lock.json

✅ This ensures teammates get the same packages automatically.

🛠️ Tips for Smooth Collaboration

Always enable Visible Meta Files and Force Text (set in Project Settings → Editor).

Avoid editing the same Scene at the same time → prefer Prefabs to split work.

Large assets (textures, audio, models) are tracked with Git LFS.

Never commit Library/ or build artifacts.

If assets appear as text pointer files (oid sha256...), run:

git lfs pull
git checkout .


✅ That’s it! Everyone should now get the exact same project setup when cloning.

# Microsoft Agent Framework—Design, Orchestrate, and Scale Agentic AI
This is the repository for the LinkedIn Learning course `Microsoft Agent Framework—Design, Orchestrate, and Scale Agentic AI`. The full course is available from [LinkedIn Learning][lil-course-url].

![lil-thumbnail-url]

## Course Description

_See the readme file in the main branch for updated instructions and information._
## Instructions

This repository contains branches for the videos in this course that involve writing code — not every video has a branch, only the ones where you are actively building something.

You can use the branch pop-up menu in GitHub to switch to a specific branch and see the code at that stage, or append `/tree/BRANCH_NAME` to the repository URL to navigate directly to a branch.

> **Recommended: Fork this repository first!**
> If you want to follow along and make your own changes, fork this repository to your own GitHub account before cloning it. That way you have your own copy to experiment with freely, without affecting the original course files.

## Branches

Branches follow the naming convention `CHAPTER#_VIDEO#`. For example, `02_03` corresponds to Chapter 2, Video 3.

Each branch represents the **starting state** of the code for that video — what you need in place *before* the video begins. The ending state of a video is simply the starting state of the next video's branch, so no separate end branch is needed for most videos.

Some videos have both a `b` (beginning) and an `e` (end) branch when the end state is not simply the start of the next video. The `main` branch holds the final complete state of the course.

| Branch | Chapter | Video | Description |
|--------|---------|-------|-------------|
| `01_02` | 1 | 2 | Building your first Agent |
| `01_03` | 1 | 3 | Giving Memory to Your Agent |
| `01_04` | 1 | 4 | Adding Power: Tools |
| `01_05b` | 1 | 5 | Going MCP — beginning state |
| `01_05e` | 1 | 5 | Going MCP — end state |
| `02_03` | 2 | 3 | Design Team Skeleton |
| `02_04` | 2 | 4 | Product Manager Agent |
| `02_05` | 2 | 5 | Architect Agent |
| `02_06` | 2 | 6 | QA Engineer Agent |
| `02_07b` | 2 | 7 | Feature Card Agent — beginning state |
| `02_07e` | 2 | 7 | Feature Card Agent — end state (complete course) |

When switching branches after making local changes, you may see an error like this:

    error: Your local changes to the following files would be overwritten by checkout: [files]
    Please commit your changes or stash them before you switch branches.
    Aborting

To resolve this:

    git add .
    git commit -m "my changes"

## Installing

All installation requirements are covered step by step in the course in **Chapter 0, Video 02 — What you should know**. The following tools and accounts are needed:

- .NET 10 SDK
- Visual Studio Code
- Azure account + Microsoft Foundry
- Node.js LTS
- GitHub account + Personal Access Token
- This repository (cloned or forked)

## Instructor

**José Luis Latorre**
*Community Lead + Agentic & Software Architect @ Swiss Life | Microsoft AI MVP | AgentEval creator | LinkedIn Learning Instructor*

José works as Dev Community Lead and Agentic & Software Architect at Swiss Life AG, with over 20 years in software engineering spanning the full spectrum from enterprise systems to modern cloud-native AI. He focuses on two things: helping developers ship better software through DevEx, standards, and practices, and building AI systems that still behave when they hit production. His specialties are Generative AI, Agentic AI engineering, and Azure cloud solutions—especially where the "cool demo" meets real constraints like security, performance, governance, and reliability. Since 2023 he has been an active contributor to Microsoft's agentic AI frameworks—Semantic Kernel, AutoGen, and now Microsoft Agent Framework (Contributor #29)—and created AgentEval, the .NET evaluation toolkit that brings validation discipline to AI agents and agentic workflows. He is a LinkedIn Learning Course Author with four released courses and around 60,000 learners, and speaks at conferences and user groups about building agentic AI in .NET: sharing real engineering lessons, practical patterns, and live demos to help teams build agents that are not just impressive, but measurably better.

Check out his other courses on [LinkedIn Learning](https://www.linkedin.com/in/joslat/).


[0]: # (Replace these placeholder URLs with actual course URLs)

[lil-course-url]: https://www.linkedin.com/learning/
[lil-thumbnail-url]: https://media.licdn.com/dms/image/v2/D4E0DAQG0eDHsyOSqTA/learning-public-crop_675_1200/B4EZVdqqdwHUAY-/0/1741033220778?e=2147483647&v=beta&t=FxUDo6FA8W8CiFROwqfZKL_mzQhYx9loYLfjN-LNjgA


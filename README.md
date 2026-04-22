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
1. To use these exercise files, you must have the following installed:
	- [list of requirements for course]
2. Clone this repository into your local machine using the terminal (Mac), CMD (Windows), or a GUI tool like SourceTree.
3. [Course-specific instructions]

## Instructor

Instructor name

Instructor description

                            

Check out my other courses on [LinkedIn Learning](https://www.linkedin.com/learning/instructors/).


[0]: # (Replace these placeholder URLs with actual course URLs)

[lil-course-url]: https://www.linkedin.com/learning/
[lil-thumbnail-url]: https://media.licdn.com/dms/image/v2/D4E0DAQG0eDHsyOSqTA/learning-public-crop_675_1200/B4EZVdqqdwHUAY-/0/1741033220778?e=2147483647&v=beta&t=FxUDo6FA8W8CiFROwqfZKL_mzQhYx9loYLfjN-LNjgA


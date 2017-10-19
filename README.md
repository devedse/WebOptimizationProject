# WebOptimizationProject

[![Join the chat at https://gitter.im/WebOptimizationProject/Lobby](https://badges.gitter.im/WebOptimizationProject/Lobby.svg)](https://gitter.im/WebOptimizationProject/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/wrq3ewhxqjbqermk?svg=true)](https://ci.appveyor.com/project/devedse/weboptimizationproject)

This project will ultimately optimize the complete web :)

## Information

This project is currently still in early development. All images in the pull requests that are submitted are tested to be 100% pixel by pixel the same as their originals. (They are just smaller :smile:).

If you have any questions, suggestions or other ideas, let me know.

## How does it work?

The Web Optimization Project makes use of DeveImageOptimizer to optimize a repository. DeveImageOptimizer uses FileOptimizerFull to optimize the images (which uses a bunch of tools like PNGOut, ECT, PNGCrush, etc). After this, DeveImageOptimizer uses ImageSharp to do a Pixel per Pixel comparison of every image that was optimized to ensure they are visually equal (lossless).

After this is done, a pull request is created with all optimized images.

## Help my repository got optimized!?!?

Currently I'm just manually choosing some github repositories to optimize, mainly to further extend the project and add more types of images to support. So if your project is one of the few that got optimized, feel free to accept my pull request and start making the web a tiny bit faster :smile:.

# Note
This is a fork of Lecoati/LeBlender to support Umbraco v8. This may result in a PullRequest for the original project.

# LeBlender

LeBlender is an open source Umbraco backoffice extension which made the Grid Canvas Editors management easier and flexible.
We can create, order, update, remove and extend Grid editors on the fly, through a very simple and nice user interface.
LeBlender project brings two main features:

## LeBlender for umbraco v8

We've gotten a lot of questions about what the plans for LeBlender and Umbraco V8 are. It’s great to see that so many of you would like to use LeBlender, and that there’s so much positive feedback about it.

We are currently investigating what the the future of LeBlender should be. We hope for your patience and will make an announcement about the future of LeBlender shortly after CodeGarden ’19.

## 1. LeBlender editor manager

This part of the project is a user interface tool for managing the grid editor settings.
Basically, it's a simple UI to edit the grid config file grid.editors.config.js. Through it, we can manage our Umbraco project's set of grid editors without manually writing any line of JSON code.
In addition, it possible to extend it with your own grid editor in a very easy way, see more information within the documentation.

## 2. LeBlender editor
LeBlender Editor is an additional editor that allow us to create complex data structures for our Grid in just a few clicks without any line of code.
Its main features are:
-100% configurable by UI
-Optional preview within the grid backend property editor
-Simple set of properties or list of them
-Any datatype can be used as LeBlenderEditor properties
-LeBlenderEditor can be cached
-Custom controllers can be used

### See it in action


## How to install
- Checkout the umbraco-v8 branch. 
- Recompile LeBlender.Extensions. The other projects will give you compiler errors which can be ignored.
- Open a console to the directory "your repo\build". Edit build.bat and remove the lines, which will push the package to nuget.
- Execute build.bat.
- Now you have a package which can be copied to a package source. Package sources can be a local directory, a network share or a nuget implementation on a web server.
- Now you can install the package in your solutions.

### Change logs

#### 2.1
- Support for Element Content Types.

#### 1.x
- See the change logs in the source repository https://github.com/Lecoati/LeBlender

# LeBlender

LeBlender is an open source Umbraco backoffice extension which made the Grid Canvas Editors management easier and flexible.
We can create, order, update, remove and extend Grid editors on the fly, through a very simple and nice user interface.
LeBlender project brings two main features:

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

See it in action https://youtu.be/gh_3bP8C28g

## How to install
Download LeBender Package
Install it through the Umbraco Backend
Documentation: LeBlender_0.9.x.pdf
Nuget package: https://www.nuget.org/packages/Lecoati.LeBlender/
Upgrade to 1.0.0
We have tried to make LeBlender 1.0.0 compatible with previous versions.
Nevertheless there are some breaking changes regarding the LeBlender editor. The most relevant is that the default LeBlender model name has changed. On the previous version it was called BlenderModel, it has been changed for LeBlenderModel.
Follow those steps to upgrade your project
Remove LeBlender Datatype
Uninstall previous version (delete "/App_plugin/Lecoati.LeBlender/" folder and "/bin/Lecoati.leblender.Extension.dll")
Install LeBlender 1.0.0
Change your BlenderModel references to LeBlenderModel
Save and publish the content which uses the LeBlender editor

### Change logs
 
#### 1.0.8
- Important Bug fixed: LeBlender Helper surfase controller return 404 since umb 7.2.7+
- Numeric property returns 0
- Dropdown can't be deselected
- Broadcast formSubmitting on closing the right side bar

#### 1.0.5 / 1.0.6 / 1.0.7
- Bugs fixing

#### 1.0.3 / 1.0.4
- Property value converter
- Bugs fixing

#### 1.0.2
- Nuget package fixed with the right plugin path

#### 1.0.1
- Bug fixed: remove empty render property

#### 1.0.0
- LeBlender as a tree
- LeBlender editor manager extension
- LeBlender editor caching
- LeBlender editor custom controller

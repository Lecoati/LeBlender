[
    {
        "name": "Carousel",
        "alias": "carousel",
        "view": "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/BlenderGridEditor.html",
        "render": "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/Views/Carousel.cshtml",
        "icon": "icon-pictures-alt-2",
        "config": {
            "limit": "10",
            "fixed": "false",
            "editors":[
                {
                    "name": "Title",
                    "alias": "title",
                    "view": "textstring"
                },
                {
                    "name": "Summary",
                    "alias": "summary",
                    "view": "textarea"
                },
                {
                    "name": "Image",
                    "alias": "image",
                    "view": "mediapicker"
                },
                {
                    "name": "Content",
                    "alias": "content",
                    "view": "contentpicker"
                }
            ]
        }   
    },
    {
        "name": "Thumbnail",
        "alias": "thumbnail",
        "view": "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/BlenderGridEditor.html",
        "render": "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/Views/Thumbnail.cshtml",
        "icon": "icon-thumbnail-list",
        "config": {
            "limit": "4",
            "fixed": "true",
            "editors":[
                {
                    "name": "Title",
                    "alias": "title",
                    "view": "textstring"
                },
                {
                    "name": "Summary",
                    "alias": "summary",
                    "view": "textarea"
                },
                {
                    "name": "Image",
                    "alias": "image",
                    "view": "mediapicker"
                },
                {
                    "name": "Content",
                    "alias": "content",
                    "view": "contentpicker"
                }
            ]
        }   
    },
    {
        "name": "Tabs",
        "alias": "tabs",
        "view": "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/BlenderGridEditor.html",
        "render": "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/Views/Tabs.cshtml",
        "icon": "icon-tab",
        "config": {
            "limit": "10",
            "fixed": "false",
            "editors": [
                {
                    "name": "Title",
                    "alias": "title",
                    "view": "textstring"
                },
                {
                    "name": "Content",
                    "alias": "content",
                    "view": "textarea"
                }
            ]
        }
    },
    {
        "name": "Jumbotron",
        "alias": "jumbotron",
        "view": "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/BlenderGridEditor.html",
        "render": "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/Views/Jumbotron.cshtml",
        "icon": "icon-certificate",
        "config": {
            "limit": "1",
            "fixed": "true",
            "editors": [
                {
                    "name": "Title",
                    "alias": "title",
                    "view": "textstring"
                },
                {
                    "name": "Summary",
                    "alias": "summary",
                    "view": "textarea"
                },
                {
                    "name": "Content",
                    "alias": "content",
                    "view": "contentpicker"
                }
            ]
        }
    },
    {
        "name": "Accordion",
        "alias": "accordion",
        "view": "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/BlenderGridEditor.html",
        "render": "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/Views/Accordion.cshtml",
        "icon": "icon-list",
        "config": {
            "limit": "10",
            "fixed": "false",
            "editors": [
                {
                    "name": "Title",
                    "alias": "title",
                    "view": "textstring"
                },
                {
                    "name": "Content",
                    "alias": "content",
                    "view": "textarea"
                }
            ]
        }
    }
]
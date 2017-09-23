function FudanMap(projectName, campusID) {
    dojo.require("esri.map");
    dojo.require("esri.tasks.query");
    var _this = this;
    this.findTask = null;
    this.fdmap = null;
    this.highlightSymbol = null;
    this.buildingGraphicsLayer = null;
    this.currentInfoWinBuildingId = null;
    var mapLayerUrls = ["http://218.193.130.178:8080/ArcGIS/rest/services/HanDan5/MapServer", "http://218.193.130.178:8080/ArcGIS/rest/services/Jiangwan/MapServer", "http://218.193.130.178:8080/ArcGIS/rest/services/Fenglin/MapServer", "http://218.193.130.178:8080/ArcGIS/rest/services/Zhangjiang/MapServer"];
    var queryTaskUrls = ["http://218.193.130.178:8080/ArcGIS/rest/services/HanDan5/MapServer/3", "http://218.193.130.178:8080/ArcGIS/rest/services/Jiangwan/MapServer/3", "http://218.193.130.178:8080/ArcGIS/rest/services/Fenglin/MapServer/3", "http://218.193.130.178:8080/ArcGIS/rest/services/Zhangjiang/MapServer/3"];
    var initExtentStrings = [{ "xmin": 752, "ymin": -3260, "xmax": 4855, "ymax": -839, "spatialReference": { "wkid": 4490} },
                            { "xmin": 3251, "ymin": -1990, "xmax": 4704, "ymax": -737, "spatialReference": { "wkid": 4214} },
                            { "xmin": 1697, "ymin": -2487, "xmax": 3429, "ymax": -753, "spatialReference": { "wkid": 4490} },
                            { "xmin": 686, "ymin": -2158, "xmax": 2608, "ymax": -689, "spatialReference": { "wkid": 4490} }
                            ]
    var FullExtentPolygons =
            [
                { "geometry": { "rings": [[[-282.2, 231.1], [5915.2, 231.1], [5915.2, -4842.1], [-282.2, -4842.1]]], "spatialReference": { "wkid": 4490} },
                    "symbol": { "color": [0, 0, 0, 0], "outline": { "color": [0, 0, 0, 0], "width": 1, "type": "esriSLS", "style": "esriSLSSolid" }, "type": "esriSFS", "style": "esriSFSSolid" }
                },
                { "geometry": { "rings": [[[-333.3, 141.4], [6988.3, 141.4], [6988.3, -2958.4], [-333.3, -2958.4]]], "spatialReference": { "wkid": 4214} },
                    "symbol": { "color": [0, 0, 0, 0], "outline": { "color": [0, 0, 0, 0], "width": 1, "type": "esriSLS", "style": "esriSLSSolid" }, "type": "esriSFS", "style": "esriSFSSolid" }
                },
                { "geometry": { "rings": [[[-243.9, 154.1], [5110.9, 154.1], [5110.9, -3225.1], [-243.9, -3225.1]]], "spatialReference": { "wkid": 4490} },
                    "symbol": { "color": [0, 0, 0, 0], "outline": { "color": [0, 0, 0, 0], "width": 1, "type": "esriSLS", "style": "esriSLSSolid" }, "type": "esriSFS", "style": "esriSFSSolid" }
                },
                { "geometry": { "rings": [[[-167.3, 141.3], [3502.3, 141.3], [3502.3, -2956.3], [-167.3, -2956.3]]], "spatialReference": { "wkid": 4490} },
                    "symbol": { "color": [0, 0, 0, 0], "outline": { "color": [0, 0, 0, 0], "width": 1, "type": "esriSLS", "style": "esriSLSSolid" }, "type": "esriSFS", "style": "esriSFSSolid" }
                }
            ]
    var buildingInfo = {};
    var buildingFlags = {};
    var bdNameAuto = new Array();
    this.init = function (campusID) {
        //create map
        var initExtent = new esri.geometry.Extent(initExtentStrings[campusID]);
        fdmap = new esri.Map("map", { extent: initExtent });

        var layer = new esri.layers.ArcGISTiledMapServiceLayer(mapLayerUrls[campusID]);
        fdmap.addLayer(layer);
        findTask = new esri.tasks.FindTask(mapLayerUrls[campusID]);

        dojo.connect(fdmap, "onLoad", function () {
            $("#mapLoading").hide();
            initFunctionality();
        });

        $(".span_l").hover(function () {
            $(this).addClass("span_over");
        },
                    function () {
                        $(this).removeClass("span_over");
                    }
                );

        $(".mapinfo_but").live("mouseover", function () {
            $(this).addClass("mapinfo_but_on");
        });
        $(".mapinfo_but").live("mouseout", function () {
            $(this).removeClass("mapinfo_but_on");
        });

        $(".mapinfo_but_close").live("click", function () {
            $(this).removeClass("mapinfo_but_close");
            $(this).addClass("mapinfo_but_open");
            $(this).attr("title", "显示右栏");
            $(this).css("right", "0px");
            $("#map_right_panel").css("right", "-310px");
            $("#map").css("margin-right", "0px");
            fdmap.resize();
            fdmap.reposition();
        });

        $(".mapinfo_but_open").live("click", function () {
            $(this).removeClass("mapinfo_but_open");
            $(this).addClass("mapinfo_but_close");
            $(this).attr("title", "收起右栏");
            $(this).css("right", "309px");
            $("#map_right_panel").css("right", "0px");
            $("#map").css("margin-right", "310px");
            fdmap.resize();
            fdmap.reposition();
        });

        $("#tabs").tabs();
        $("#building_list").accordion({ collapsible: true, autoHeight: false, fillSpace: true, active: false });
        //获取该校区下的所有建筑信息（buildingID, buildingName）
        $.getJSON(projectName + "Admin/Global/GetBuildingsOfSchool", { schoolId: parseInt(campusID) + 1 }, function (jsonData) {
            for (var item in jsonData) {
                buildingInfo[jsonData[item].BDI_ID] = jsonData[item].BDI_Name;
                buildingFlags[jsonData[item].BDI_ID] = jsonData[item].BDI_Flag;
                bdNameAuto.push({ value: jsonData[item].BDI_Name, buildingID: jsonData[item].BDI_ID });
            }
            $("#ToolPoiSearch").autocomplete({ source: bdNameAuto });
        });
    }

    function initFunctionality() {
        //build query task
        var queryTask = new esri.tasks.QueryTask(queryTaskUrls[campusID]);

        //build query filter
        var query = new esri.tasks.Query();
        query.returnGeometry = true;
        query.outFields = ["*"];
        query.where = "Id > 0";
        var symbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID, new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_DASHDOT, new dojo.Color([255, 0, 0, 0]), 2), new dojo.Color([0, 0, 255, 0]));
        highlightSymbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID, new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([255, 255, 0]), 2), new dojo.Color([64, 0, 128, 0.35]));
        var fullExtentGraphic = new esri.Graphic(FullExtentPolygons[campusID]);
        var fullExtentGraphicLayer = new esri.layers.GraphicsLayer();
        fullExtentGraphicLayer.add(fullExtentGraphic);
        fdmap.addLayer(fullExtentGraphicLayer);
        fullExtentGraphicLayer.enableMouseEvents();

        dojo.connect(queryTask, "onComplete", function (featureSet) {
            buildingGraphicsLayer = new esri.layers.GraphicsLayer();
            for (var i = 0, il = featureSet.features.length; i < il; i++) {
                var graphic = featureSet.features[i];
                graphic.setSymbol(symbol);
                buildingGraphicsLayer.add(graphic);
            }
            fdmap.addLayer(buildingGraphicsLayer);
            fdmap.graphics.enableMouseEvents();
            buildingGraphicsLayer.enableMouseEvents();

            dojo.connect(buildingGraphicsLayer, "onMouseOver", function (evt) {
                fdmap.graphics.clear();
                var graphic = evt.graphic;
                var attr = graphic.attributes;

                var highlightGraphic = new esri.Graphic(graphic.geometry, highlightSymbol);
                highlightGraphic.setAttributes(attr);
                fdmap.graphics.add(highlightGraphic);
                if (buildingInfo[attr.Id] != null) {
                    $("#buildingNameTip").html(buildingInfo[attr.Id]);
                }
                else {
                    $("#buildingNameTip").html("暂无命名");
                }

                var tip = $("#map").offset();
                $("#buildingNameTip").css({ position: "absolute", left: evt.clientX - tip.left, top: evt.clientY - tip.top + document.documentElement.scrollTop + document.body.scrollTop });
                $("#buildingNameTip").show();
                fdmap.setMapCursor("pointer");
            });
        });

        dojo.connect(fdmap.graphics, "onMouseOut", function (evt) {
            fdmap.setMapCursor("default");
            if (!fdmap.infoWindow.isShowing) {
                fdmap.graphics.clear();
                $("#buildingNameTip").hide();
            }
        });
        dojo.connect(fullExtentGraphicLayer, "onClick", function () {
            fdmap.infoWindow.hide();
            fdmap.graphics.clear();
            $("#buildingNameTip").hide();
            fdmap.setMapCursor("default");
        });
        dojo.connect(fdmap.infoWindow, "onHide", function () {
            fdmap.graphics.clear();
            $("#buildingNameTip").hide();
            fdmap.setMapCursor("default");
        });
        dojo.connect(fdmap, "onZoom", function (evt) {
            fdmap.setMapCursor("default");
        });
        queryTask.execute(query);

        $(window).resize(function () {
            fdmap.resize();
            fdmap.reposition();
        });
    }

    this.showInfoOnClick = function () {
        dojo.connect(fdmap, "onLoad", function () {
            dojo.connect(fdmap.graphics, "onClick", function (evt) {
                var graphic = fdmap.graphics.graphics[0];
                var attr = graphic.attributes;
                var buildingName = null;
                currentInfoWinBuildingId = attr.Id;
                if (buildingFlags[currentInfoWinBuildingId] == 0) return;
                $.getJSON(projectName + "Admin/Global/GetRecentEnergyAjax", {
                    buildingId: currentInfoWinBuildingId, powerType: "001", timeRange: "02"
                }, function (jsonData) {
                    var content = $("#infoWindowContent").html();
                    fdmap.infoWindow.resize(390, 360);
                    fdmap.infoWindow.setTitle('<div style=\"line-height: 30px;  overflow: hidden; height: 30px; white-space: nowrap; width: 345px;\" class=\"BMap_bubble_title\"><p  class=\"iw_poi_title\">' + jsonData.buildingName + '<a target="_blank" onclick=\"\" href=\"' + projectName + "Admin/Global/BuildingDetail?campusID=" + campusID + "&buildingID=" + currentInfoWinBuildingId + '\">详情»</a></p></div>');
                    fdmap.infoWindow.setContent(content);
                    fdmap.infoWindow.show(evt.screenPoint, fdmap.getInfoWindowAnchor(evt.screenPoint));
                    var chartOtherData = { title: "最近每天用电量", yAxisTitle: "用电量（度）", renderType: "Column" };
                    var data = $.extend(jsonData, chartOtherData);
                    var chartXML = util_templateToChartXml("chartTemplate", data);
                    var statisChart = new Visifire(projectName + "Content/sl/SL.Visifire.Charts.xap", "StatisticsChart", 370, 200, "White");
                    statisChart.setDataXml(chartXML);
                    statisChart.render("chartContainer");
                    if (jsonData.energyUsedYear != 0) {
                        $("#energyUsedSpan").html(jsonData.energyUsedYear);
                    } else {
                        $("#energyUsedSpan").html("暂缺");
                    }
                    if (jsonData.energyRemain != 0) {
                        $("#energyRemainSpan").html(jsonData.energyRemain);
                    } else {
                        $("#energyRemainSpan").html("暂缺");
                    }
                });
            });
        })
    };

    this.reflashOnClick = function () {
        dojo.connect(fdmap, "onLoad", function () {
            dojo.connect(fdmap.graphics, "onClick", function (evt) {
                var graphic = fdmap.graphics.graphics[0];
                var attr = graphic.attributes;
                currentInfoWinBuildingId = attr.Id;
                window.location.href = projectName + "Admin/Global/BuildingDetail?campusID=" + campusID + "&buildingID=" + currentInfoWinBuildingId;
            })
        });
    };

    this.highlightBuilding = function (buildingId) {
        if (fdmap.graphics != null) {
            fdmap.graphics.clear();
        }
        //find the polygon
        var ploygonFindParams = new esri.tasks.FindParameters();
        ploygonFindParams.returnGeometry = true;
        ploygonFindParams.layerIds = [3];
        ploygonFindParams.searchFields = ["Id"];
        ploygonFindParams.searchText = buildingId;
        findTask.execute(ploygonFindParams, function (results) {
            for (var i = 0; i < results.length; i++) {
                var currentFeature = results[i];
                var graphic = currentFeature.feature;
                graphic.setSymbol(highlightSymbol);
                var mapcenterPoint = graphic.geometry.getExtent().getCenter();
                var centerPoint = fdmap.toScreen(mapcenterPoint);
                fdmap.graphics.add(graphic);
                if (campusID != 1) {
                    fdmap.centerAndZoom(mapcenterPoint, 4);
                }
                else {
                    fdmap.centerAndZoom(mapcenterPoint, 7);
                }
                setTimeout(function () {
                    if (buildingInfo[buildingId] != null) {
                        $("#buildingNameTip").html(buildingInfo[buildingId]);
                    }
                    else {
                        $("#buildingNameTip").html("暂无命名");
                    }
                    var centerPoint = esri.geometry.toScreenPoint(fdmap.extent, fdmap.width, fdmap.height, mapcenterPoint);
                    $("#buildingNameTip").css({ position: "absolute", left: centerPoint.x, top: centerPoint.y });
                    $("#buildingNameTip").show();
                }, 1000);
            }
        });
    };
    function highlightBuilding2(buildingId) {
        if (fdmap.graphics != null) {
            fdmap.graphics.clear();
        }
        //find the polygon
        var ploygonFindParams = new esri.tasks.FindParameters();
        ploygonFindParams.returnGeometry = true;
        ploygonFindParams.layerIds = [3];
        ploygonFindParams.searchFields = ["Id"];
        ploygonFindParams.searchText = buildingId;
        findTask.execute(ploygonFindParams, function (results) {
            for (var i = 0; i < results.length; i++) {
                var currentFeature = results[i];
                var graphic = currentFeature.feature;
                graphic.setSymbol(highlightSymbol);
                var mapcenterPoint = graphic.geometry.getExtent().getCenter();
                var centerPoint = fdmap.toScreen(mapcenterPoint);
                fdmap.graphics.add(graphic);
                if (campusID != 1) {
                    fdmap.centerAndZoom(mapcenterPoint, 4);
                }
                else {
                    fdmap.centerAndZoom(mapcenterPoint, 7);
                }
                setTimeout(function () {
                    if (buildingInfo[buildingId] != null) {
                        $("#buildingNameTip").html(buildingInfo[buildingId]);
                    }
                    else {
                        $("#buildingNameTip").html("暂无命名");
                    }
                    var centerPoint = esri.geometry.toScreenPoint(fdmap.extent, fdmap.width, fdmap.height, mapcenterPoint);
                    $("#buildingNameTip").css({ position: "absolute", left: centerPoint.x, top: centerPoint.y });
                    $("#buildingNameTip").show();
                }, 1000);
            }
        });
    };
    this.searchBuilding = function (searchText) {
        $.getJSON(projectName + "Admin/Global/GetBuilding", { buildingName: searchText }, function (jsonData) {
            if (jsonData.length == 1) {
                highlightBuilding2(jsonData[0].BuildingID);
                $(".go_back_clear span").html("共找到" + jsonData.length + "个结果");
                $("#building_search_res tbody").html($("#searchResultTemplate").tmpl(jsonData));
            }
            else if (jsonData.length == 0) {
                $(".go_back_clear span").html("对不起，没有找到名称中包含\"" + searchText + "\"的建筑。");
                $("#building_search_res tbody").html("");
            }
            else {
                $(".go_back_clear span").html("共找到" + jsonData.length + "个结果");
                $("#building_search_res tbody").html($("#searchResultTemplate").tmpl(jsonData));
            }
            $("#tabs").tabs("select", 1);
        });
    };

    this.searchSubmitKeyClick = function (searchText, evt) {
        evt = (evt) ? evt : ((window.event) ? window.event : "")
        keyCode = evt.keyCode ? evt.keyCode : (evt.which ? evt.which : evt.charCode);
        if (keyCode == 13) {
            this.searchBuilding(searchText);
        }
    };


    this.activeBuilding = function (buildingId) {
        dojo.connect(fdmap, "onLoad", function () {
            _this.highlightBuilding(buildingId);
        });
    };

    this.chartUpdate = function (obj) {
        var powerType;
        var timeRange;
        var newTitlePower, newTitleTime, newYAxisTitle;
        if (obj.typeRadio[0].checked) {
            powerType = "001";
            newTitlePower = "用电量（度）";
        }
        else if (obj.typeRadio[1].checked) {
            powerType = "002";
            newTitlePower = "用水量（吨）"
        }
        else if (obj.typeRadio[2].checked) {
            powerType = "003";
            newTitlePower = "用气量（立方米）"
        }
        if (obj.gradingRadio[1].checked) {
            timeRange = "02";
            newTitleTime = "十天";
        }
        else if (obj.gradingRadio[2].checked) {
            timeRange = "01";
            newTitleTime = "月度";
        } else if (obj.gradingRadio[0].checked) {
            timeRange = "03";
            newTitleTime = "24小时";
        }
        $.getJSON(projectName + "Admin/Global/GetRecentEnergyAjax", { buildingId: currentInfoWinBuildingId, powerType: powerType, timeRange: timeRange },
            function (jsonData) {
                var chartOtherData = { title: "最近" + newTitleTime + newTitlePower, yAxisTitle: newTitlePower, renderType: "Column" };
                var data = $.extend(jsonData, chartOtherData);
                var chartXML = util_templateToChartXml("chartTemplate", data);
                var statisChart = new Visifire("../../../../Content/sl/SL.Visifire.Charts.xap", "StatisticsChart", 370, 200, "White");
                statisChart.setDataXml(chartXML);
                statisChart.render("chartContainer");
                $(".energyUsedTitle").html(newTitlePower);
                if (jsonData.energyUsedYear != 0) {
                    $("#energyUsedSpan").html(jsonData.energyUsedYear);
                } else {
                    $("#energyUsedSpan").html("暂缺");
                }

                if (jsonData.energyRemain != 0) {
                    $("#energyRemainSpan").html(jsonData.energyRemain);
                } else {
                    $("#energyRemainSpan").html("暂缺");
                }
            })
    }
}
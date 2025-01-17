namespace Plotly.NET

open System
open System.IO

open GenericChart
open System.Runtime.InteropServices
open System.Runtime.CompilerServices

/// Extensions methods for Charts supporting the fluent pipeline style 'Chart.WithXYZ(...)'.
[<AutoOpen>]

module ChartExtensions =

    ///Choose process to open plots with depending on OS. Thanks to @zyzhu for hinting at a solution (https://github.com/plotly/Plotly.NET/issues/31)
    let internal openOsSpecificFile path =
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let psi = new System.Diagnostics.ProcessStartInfo(FileName = path, UseShellExecute = true)
            System.Diagnostics.Process.Start(psi) |> ignore
        elif RuntimeInformation.IsOSPlatform(OSPlatform.Linux) then
            System.Diagnostics.Process.Start("xdg-open", path) |> ignore
        elif RuntimeInformation.IsOSPlatform(OSPlatform.OSX) then
            System.Diagnostics.Process.Start("open", path) |> ignore
        else
            invalidOp "Not supported OS platform"

    open Trace

    /// Provides a set of static methods for creating charts.
    type Chart with

// ############################################################
// ####################### Apply to trace
        
        /// Set the name related properties of a trace
        [<CompiledName("WithTraceName")>]
        static member withTraceName
            (
                [<Optional;DefaultParameterValue(null)>] ?Name,
                [<Optional;DefaultParameterValue(null)>] ?Showlegend,
                [<Optional;DefaultParameterValue(null)>] ?Legendgroup,
                [<Optional;DefaultParameterValue(null)>] ?Visible
            ) =
                fun (ch:GenericChart) ->
                    ch |> mapiTrace (fun i trace ->
                        let naming i name = name |> Option.map (fun v -> if i = 0 then v else sprintf "%s_%i" v i)
                        trace 
                        |> TraceStyle.TraceInfo(?Name=(naming i Name),?Showlegend=Showlegend,?Legendgroup=Legendgroup,?Visible=Visible)
                    )

         /// Set the axis anchor id the trace is belonging to
        [<CompiledName("WithAxisAnchor")>]
        static member withAxisAnchor
            (
                [<Optional;DefaultParameterValue(null)>] ?X,
                [<Optional;DefaultParameterValue(null)>] ?Y,
                [<Optional;DefaultParameterValue(null)>] ?Z
            ) =
                let idx   = if X.IsSome then Some (StyleParam.AxisAnchorId.X X.Value) else None
                let idy   = if Y.IsSome then Some (StyleParam.AxisAnchorId.Y Y.Value) else None
                let idz   = if Z.IsSome then Some (StyleParam.AxisAnchorId.Z Z.Value) else None
            
                fun (ch:GenericChart) ->
                    ch |> mapTrace (fun trace ->
                        trace 
                        |> TraceStyle.SetAxisAnchor(?X=idx,?Y=idy,?Z=idz)
                    )
        [<CompiledName("WithAxisAnchor")>]
        static member withAxisAnchor
            (
                (ch:GenericChart),
                [<Optional;DefaultParameterValue(null)>] ?X,
                [<Optional;DefaultParameterValue(null)>] ?Y,
                [<Optional;DefaultParameterValue(null)>] ?Z
            ) =
                ch |> Chart.withAxisAnchor(?X=X,?Y=Y,?Z=Z)
  
        /// Apply styling to the Marker(s) of the chart as Object.
        [<CompiledName("WithMarker")>]
        static member withMarker(marker:Marker) =
            (fun (ch:GenericChart) ->
                ch |> mapTrace (TraceStyle.SetMarker(marker))
            )

        /// Apply styling to the Marker(s) of the chart.
        [<CompiledName("WithMarkerStyle")>]
        static member withMarkerStyle
            (
                [<Optional;DefaultParameterValue(null)>] ?Size,
                [<Optional;DefaultParameterValue(null)>] ?Color,
                [<Optional;DefaultParameterValue(null)>] ?Symbol,
                [<Optional;DefaultParameterValue(null)>] ?Opacity
            ) =
                let marker = 
                    Marker.init ( 
                        ?Size=Size,?Color=Color,?Symbol=Symbol,?Opacity=Opacity
                        )
                Chart.withMarker(marker)

        /// Apply styling to the Line(s) of the chart as Object.
        [<CompiledName("WithLine")>]
        static member withLine(line:Line) =
             (fun (ch:GenericChart) ->
                ch |> mapTrace (TraceStyle.SetLine(line))
            )

        /// Apply styling to the Line(s) of the chart.
        [<CompiledName("WithLineStyle")>]
        static member withLineStyle
            (
                [<Optional;DefaultParameterValue(null)>] ?Width,
                [<Optional;DefaultParameterValue(null)>] ?Color,
                [<Optional;DefaultParameterValue(null)>] ?Shape,
                [<Optional;DefaultParameterValue(null)>] ?Dash,
                [<Optional;DefaultParameterValue(null)>] ?Smoothing,
                [<Optional;DefaultParameterValue(null)>] ?Colorscale
            ) =
                let line =
                    Line.init (
                        ?Width=Width,?Color=Color,?Shape=Shape,?Dash=Dash,?Smoothing=Smoothing,?Colorscale=Colorscale)

                Chart.withLine(line)

        /// Apply styling to the xError(s) of the chart as Object
        [<CompiledName("WithXError")>]
        static member withXError(xError:Error) =
            (fun (ch:GenericChart) ->
                ch |> mapTrace (TraceStyle.SetErrorX(xError))
            ) 

        /// Apply styling to the xError(s) of the chart as Object
        [<CompiledName("WithXErrorStyle")>]
        static member withXErrorStyle
            (
                [<Optional;DefaultParameterValue(null)>] ?Array,
                [<Optional;DefaultParameterValue(null)>] ?Arrayminus,
                [<Optional;DefaultParameterValue(null)>] ?Symmetric,
                [<Optional;DefaultParameterValue(null)>] ?Color,
                [<Optional;DefaultParameterValue(null)>] ?Thickness,
                [<Optional;DefaultParameterValue(null)>] ?Width
            ) =
                let error = Error.init(?Array=Array,?Arrayminus=Arrayminus,?Symmetric=Symmetric,?Color=Color,?Thickness=Thickness,?Width=Width)
                Chart.withXError error

        /// Apply styling to the yError(s) of the chart as Object
        [<CompiledName("WithYError")>]
        static member withYError(yError:Error) =
            (fun (ch:GenericChart) ->
                ch |> mapTrace (TraceStyle.SetErrorY(yError))
            )

        /// Apply styling to the yError(s) of the chart as Object
        [<CompiledName("WithYErrorStyle")>]
        static member withYErrorStyle
            (
                [<Optional;DefaultParameterValue(null)>] ?Array,
                [<Optional;DefaultParameterValue(null)>] ?Arrayminus,
                [<Optional;DefaultParameterValue(null)>] ?Symmetric,
                [<Optional;DefaultParameterValue(null)>] ?Color,
                [<Optional;DefaultParameterValue(null)>] ?Thickness,
                [<Optional;DefaultParameterValue(null)>] ?Width
            ) =
                let error = Error.init(?Array=Array,?Arrayminus=Arrayminus,?Symmetric=Symmetric,?Color=Color,?Thickness=Thickness,?Width=Width)
                Chart.withYError error

        /// Apply styling to the zError(s) of the chart as Object
        [<CompiledName("WithZError")>]
        static member withZError(zError:Error) =
            (fun (ch:GenericChart) ->
                ch |> mapTrace (TraceStyle.SetErrorZ(zError))
            )
        
        /// Apply styling to the zError(s) of the chart as Object
        [<CompiledName("WithZErrorStyle")>]
        static member withZErrorStyle
            (
                [<Optional;DefaultParameterValue(null)>] ?Array,
                [<Optional;DefaultParameterValue(null)>] ?Arrayminus,
                [<Optional;DefaultParameterValue(null)>] ?Symmetric,
                [<Optional;DefaultParameterValue(null)>] ?Color,
                [<Optional;DefaultParameterValue(null)>] ?Thickness,
                [<Optional;DefaultParameterValue(null)>] ?Width
            ) =
                let error = Error.init(?Array=Array,?Arrayminus=Arrayminus,?Symmetric=Symmetric,?Color=Color,?Thickness=Thickness,?Width=Width)
                Chart.withZError error


// ############################################################
// ####################### Apply to layout
        
        // Sets x-Axis of 2d and 3d- Charts
        [<CompiledName("WithX_Axis")>]
        static member withX_Axis(xAxis:Axis.LinearAxis,[<Optional;DefaultParameterValue(null)>] ?Id) =
            (fun (ch:GenericChart) ->
                let contains3d =
                    ch 
                    |> existsTrace (fun t ->
                        match t with
                        | :? Trace3d -> true
                        | _ -> false)

                match contains3d with
                | false -> 
                    let layout =
                        let id = if Id.IsSome then StyleParam.AxisId.X Id.Value else StyleParam.AxisId.X 1
                        GenericChart.getLayout ch 
                        |> Layout.UpdateLinearAxisById(id,axis=xAxis)
                    GenericChart.setLayout layout ch
                | true  -> 
                    let layout =
                        Layout() 
                        |> Layout.style (Scene=Scene.init( xAxis=xAxis) )
                    GenericChart.addLayout layout ch
            )


        // Sets x-Axis of 2d and 3d- Charts
        [<CompiledName("WithX_AxisStyle")>]
        static member withX_AxisStyle(title,
                [<Optional;DefaultParameterValue(null)>] ?MinMax,
                [<Optional;DefaultParameterValue(null)>] ?Showgrid,
                [<Optional;DefaultParameterValue(null)>] ?Showline,
                [<Optional;DefaultParameterValue(null)>] ?Side,
                [<Optional;DefaultParameterValue(null)>] ?Overlaying,
                [<Optional;DefaultParameterValue(null)>] ?Id,
                [<Optional;DefaultParameterValue(null)>] ?Domain,
                [<Optional;DefaultParameterValue(null)>] ?Position,
                [<Optional;DefaultParameterValue(null)>] ?Zeroline,
                [<Optional;DefaultParameterValue(null)>] ?Anchor) =
            let range  = if MinMax.IsSome then Some (StyleParam.Range.MinMax (MinMax.Value)) else None
            let domain = if Domain.IsSome then Some (StyleParam.Range.MinMax (Domain.Value)) else None
            let xaxis  = Axis.LinearAxis.init(Title=title,?Range=range,?Showgrid=Showgrid,?Showline=Showline,
                                    ?Anchor=Anchor,?Side=Side,?Domain=domain,?Overlaying=Overlaying,?Position=Position,?Zeroline=Zeroline)
            Chart.withX_Axis(xaxis,?Id=Id)

        /// Sets the range slider for the xAxis
        [<CompiledName("WithX_AxisRangeSlider")>]
        static member withX_AxisRangeSlider(rangeSlider:RangeSlider,
                [<Optional;DefaultParameterValue(null)>] ?Id) =
            let xaxis  = Axis.LinearAxis.init(RangeSlider = rangeSlider)
            Chart.withX_Axis(xaxis,?Id=Id)

        // Sets y-Axis of 2d and 3d- Charts
        [<CompiledName("WithY_Axis")>]
        static member withY_Axis(yAxis:Axis.LinearAxis,[<Optional;DefaultParameterValue(null)>] ?Id) =
            (fun (ch:GenericChart) ->
                let contains3d =
                    ch 
                    |> existsTrace (fun t -> 
                        match t with
                        | :? Trace3d -> true
                        | _ -> false)

                match contains3d with
                | false -> 
                    let layout =
                        let id = if Id.IsSome then StyleParam.AxisId.Y Id.Value else StyleParam.AxisId.Y 1
                        GenericChart.getLayout ch 
                        |> Layout.UpdateLinearAxisById(id,axis=yAxis)
                    GenericChart.setLayout layout ch
                | true  -> 
                    let layout =
                        Layout() 
                        |> Layout.style(Scene=Scene.init(yAxis=yAxis) )
                    GenericChart.addLayout layout ch
            )


         // Sets y-Axis of 3d- Charts
        [<CompiledName("WithY_AxisStyle")>]
        static member withY_AxisStyle(title,
                [<Optional;DefaultParameterValue(null)>] ?MinMax,
                [<Optional;DefaultParameterValue(null)>] ?Showgrid,
                [<Optional;DefaultParameterValue(null)>] ?Showline,
                [<Optional;DefaultParameterValue(null)>] ?Side,
                [<Optional;DefaultParameterValue(null)>] ?Overlaying,
                [<Optional;DefaultParameterValue(null)>] ?Id,
                [<Optional;DefaultParameterValue(null)>] ?Domain,
                [<Optional;DefaultParameterValue(null)>] ?Position,
                [<Optional;DefaultParameterValue(null)>] ?Zeroline,
                [<Optional;DefaultParameterValue(null)>] ?Anchor) =
            let range  = if MinMax.IsSome then Some (StyleParam.Range.MinMax (MinMax.Value)) else None
            let domain = if Domain.IsSome then Some (StyleParam.Range.MinMax (Domain.Value)) else None
            let yaxis  = Axis.LinearAxis.init(Title=title,?Range=range,?Showgrid=Showgrid,
                                    ?Showline=Showline,?Anchor=Anchor,?Side=Side,?Domain=domain,?Overlaying=Overlaying,?Position=Position,?Zeroline=Zeroline)
            Chart.withY_Axis(yaxis,?Id=Id)                


        // Sets z-Axis of 3d- Charts
        [<CompiledName("WithZ_Axis")>]
        static member withZ_Axis(zAxis:Axis.LinearAxis) =
            (fun (ch:GenericChart) ->
                let layout =
                    Layout() 
                    |> Layout.style(Scene=Scene.init(zAxis=zAxis) )
                GenericChart.addLayout layout ch
             )


        // Sets z-Axis style with ...
        [<CompiledName("WithZ_AxisStyle")>]
        static member withZ_AxisStyle(title,
                [<Optional;DefaultParameterValue(null)>] ?MinMax,
                [<Optional;DefaultParameterValue(null)>] ?Showgrid,
                [<Optional;DefaultParameterValue(null)>] ?Showline,
                [<Optional;DefaultParameterValue(null)>] ?Domain,
                [<Optional;DefaultParameterValue(null)>] ?Anchor) =
            let range  = if MinMax.IsSome then Some (StyleParam.Range.MinMax (MinMax.Value)) else None
            let domain = if Domain.IsSome then Some (StyleParam.Range.MinMax (Domain.Value)) else None
            let zaxis  = Axis.LinearAxis.init(Title=title,?Range=range,?Showgrid=Showgrid,?Showline=Showline,?Anchor=Anchor,?Domain=domain)
            Chart.withZ_Axis(zaxis)

        [<CompiledName("WithColorBar")>]
        static member withColorBar(colorbar:Colorbar) =
            (fun (ch:GenericChart) ->
                ch
                |> GenericChart.mapTrace(fun t ->
                    colorbar |> DynObj.setValue t "colorbar" 
                    t
                )
            )

        
        [<CompiledName("withColorbar")>]
        static member withColorBarStyle(title,
                [<Optional;DefaultParameterValue(null)>] ?TitleSide: StyleParam.Side,
                [<Optional;DefaultParameterValue(null)>] ?TitleFont: Font,
                [<Optional;DefaultParameterValue(null)>] ?Length,
                [<Optional;DefaultParameterValue(null)>] ?OutlineColor,
                [<Optional;DefaultParameterValue(null)>] ?BorderColor,
                [<Optional;DefaultParameterValue(null)>] ?BGColor) =
            let colorbar = Colorbar.init(Title=title,?Titleside=TitleSide,?Titlefont=TitleFont,?Len = Length,?Outlinecolor=OutlineColor,?Bgcolor=BGColor,?Bordercolor=BorderColor)
            Chart.withColorBar(colorbar)
        //// Sets second x-Axis of 2d- Charts
        //static member withX_Axis2(xAxis2:Axis.LinearAxis) =
        //    (fun (ch:GenericChart) ->
        //            let layout =
        //                GenericChart.getLayout ch 
        //                |> Layout.style (xAxis2=xAxis2)
        //            GenericChart.setLayout layout ch
        //            )


        // // Sets second x-Axis of 2d- Charts
        //static member withX_Axis2Style(title,?MinMax,?Showgrid,?Showline) =
        //    let range = if MinMax.IsSome then Some (StyleParam.Range.MinMax (MinMax.Value)) else None
        //    let xaxis = Axis.LinearAxis.init(Title=title,?Range=range,?Showgrid=Showgrid,?Showline=Showline,Side=StyleParam.Side.Top)
        //    Chart.withX_Axis2(xaxis)


        //// Sets second y-Axis of 2d- Charts
        //static member withY_Axis2(yAxis2:Axis.LinearAxis) =
        //    (fun (ch:GenericChart) ->
        //            let layout =
        //                GenericChart.getLayout ch 
        //                |> Layout.style (yAxis2=yAxis2)
        //            GenericChart.setLayout layout ch
        //            )


        // // Sets second x-Axis of 2d- Charts
        //static member withY_Axis2Style(title,?MinMax,?Showgrid,?Showline) =
        //    let range = if MinMax.IsSome then Some (StyleParam.Range.MinMax (MinMax.Value)) else None
        //    let yaxis = Axis.LinearAxis.init(Title=title,?Range=range,?Showgrid=Showgrid,?Showline=Showline,Side=StyleParam.Side.Right)
        //    Chart.withY_Axis2(yaxis)


        // Set the Layout options of a Chart
        [<CompiledName("WithLayout")>]
        static member withLayout(layout:Layout) =
            (fun (ch:GenericChart) -> 
                GenericChart.addLayout layout ch) 

        // Set the LayoutGrid options of a Chart
        [<CompiledName("WithLayoutGrid")>]
        static member withLayoutGrid(layoutGrid:LayoutGrid) =
            (fun (ch:GenericChart) -> 
                let layout = 
                    GenericChart.getLayout ch
                    |> Layout.SetLayoutGrid layoutGrid 
                GenericChart.setLayout layout ch) 

        // Set the LayoutGrid options of a Chart
        [<CompiledName("WithLegend")>]
        static member withLegend(legend:Legend) =
            (fun (ch:GenericChart) -> 
                let layout = 
                    GenericChart.getLayout ch
                    |> Layout.setLegend legend 
                GenericChart.setLayout layout ch) 

        /// Sets a map for the given chart (will only work with traces supporting geo, e.g. choropleth, scattergeo)
        [<CompiledName("WithMap")>]
        static member withMap(map:Geo,[<Optional;DefaultParameterValue(null)>] ?Id ) =
            (fun (ch:GenericChart) ->
                let layout =
                    let id = defaultArg Id 1
                    GenericChart.getLayout ch 
                    |> Layout.UpdateMapById(id,map)
                GenericChart.setLayout layout ch
            )

        /// Sets a mapbox for the given chart (will only work with traces supporting mapboxes, e.g. choroplethmapbox, scattermapbox)
        [<CompiledName("WithMapbox")>]
        static member withMapbox(mapBox:Mapbox,[<Optional;DefaultParameterValue(null)>] ?Id ) =
            (fun (ch:GenericChart) ->
                let layout =
                    let id = defaultArg Id 1
                    GenericChart.getLayout ch 
                    |> Layout.UpdateMapboxById(id,mapBox)
                GenericChart.setLayout layout ch
            )

        /// Sets the map style for the given chart (will only work with traces supporting geo, e.g. choropleth, scattergeo)
        ///
        /// Parameters      :
        ///
        /// FitBounds       : Determines if and how this subplot's view settings are auto-computed to fit trace data
        ///
        /// Resolution      : Sets the resolution of the base layers
        ///
        /// Scope           : Set the scope of the map.
        ///
        /// Projection      : Determines the type of projection used to display the map
        ///
        /// Center          : Sets the (lon,lat) coordinates of the map's center. By default, the map's longitude center lies at the middle of the longitude range for scoped projection and above `projection.rotation.lon` otherwise. For all projection types, the map's latitude center lies at the middle of the latitude range by default.
        ///
        /// Visible         : Wether or not the base layers are visible
        ///
        /// Domain          : The domain of this geo subplot
        ///
        /// ShowCoastLine   : Sets whether or not the coastlines are drawn.
        ///
        /// CoastLineColor  : Sets the coastline color.
        ///
        /// CoastLineWidth  : Sets the coastline stroke width (in px).
        ///
        /// ShowLand        : Sets whether or not land masses are filled in color.
        ///
        /// LandColor       : Sets the land mass color.
        ///
        /// ShowOcean       : Sets whether or not oceans are filled in color.
        ///
        /// OceanColor      : Sets the ocean color
        ///
        /// ShowLakes       : Sets whether or not lakes are drawn.
        ///
        /// LakeColor       : Sets the color of the lakes.
        ///
        /// ShowRivers      : Sets whether or not rivers are drawn.
        ///
        /// RiverColor      : Sets color of the rivers.
        ///
        /// RiverWidth      : Sets the stroke width (in px) of the rivers.
        ///
        /// ShowCountries   : Sets whether or not country boundaries are drawn.
        ///
        /// CountryColor    : Sets line color of the country boundaries.
        ///
        /// CountryWidth    : Sets line width (in px) of the country boundaries.
        ///
        /// ShowSubunits    : Sets whether or not boundaries of subunits within countries (e.g. states, provinces) are drawn.
        ///
        /// SubunitColor    : Sets the color of the subunits boundaries.
        ///
        /// SubunitWidth    : Sets the stroke width (in px) of the subunits boundaries.
        ///
        /// ShowFrame       : Sets whether or not a frame is drawn around the map.
        ///
        /// FrameColor      : Sets the color the frame.
        ///
        /// FrameWidth      : Sets the stroke width (in px) of the frame.
        ///
        /// BgColor         : Set the background color of the map
        ///
        /// LatAxis         : Sets the latitudinal axis for this geo trace
        ///
        /// LonAxis         : Sets the longitudinal axis for this geo trace
        [<CompiledName("WithMapStyle")>]
        static member withMapStyle([<Optional;DefaultParameterValue(null)>] ?Id,
            [<Optional;DefaultParameterValue(null)>]?FitBounds       : StyleParam.GeoFitBounds,
            [<Optional;DefaultParameterValue(null)>]?Resolution      : StyleParam.GeoResolution,
            [<Optional;DefaultParameterValue(null)>]?Scope           : StyleParam.GeoScope,
            [<Optional;DefaultParameterValue(null)>]?Projection      : GeoProjection,
            [<Optional;DefaultParameterValue(null)>]?Center          : (float*float),
            [<Optional;DefaultParameterValue(null)>]?Visible         : bool,
            [<Optional;DefaultParameterValue(null)>]?Domain          : Domain,
            [<Optional;DefaultParameterValue(null)>]?ShowCoastLines  : bool,
            [<Optional;DefaultParameterValue(null)>]?CoastLineColor,
            [<Optional;DefaultParameterValue(null)>]?CoastLineWidth  : float,
            [<Optional;DefaultParameterValue(null)>]?ShowLand        : bool,
            [<Optional;DefaultParameterValue(null)>]?LandColor,
            [<Optional;DefaultParameterValue(null)>]?ShowOcean       : bool,
            [<Optional;DefaultParameterValue(null)>]?OceanColor,
            [<Optional;DefaultParameterValue(null)>]?ShowLakes       : bool,
            [<Optional;DefaultParameterValue(null)>]?LakeColor,
            [<Optional;DefaultParameterValue(null)>]?ShowRivers      : bool,
            [<Optional;DefaultParameterValue(null)>]?RiverColor,
            [<Optional;DefaultParameterValue(null)>]?RiverWidth      : float,
            [<Optional;DefaultParameterValue(null)>]?ShowCountries   : bool,
            [<Optional;DefaultParameterValue(null)>]?CountryColor,
            [<Optional;DefaultParameterValue(null)>]?CountryWidth    : float,
            [<Optional;DefaultParameterValue(null)>]?ShowSubunits    : bool,
            [<Optional;DefaultParameterValue(null)>]?SubunitColor,
            [<Optional;DefaultParameterValue(null)>]?SubunitWidth    : float,
            [<Optional;DefaultParameterValue(null)>]?ShowFrame       : bool,
            [<Optional;DefaultParameterValue(null)>]?FrameColor,
            [<Optional;DefaultParameterValue(null)>]?FrameWidth      : float,
            [<Optional;DefaultParameterValue(null)>]?BgColor,
            [<Optional;DefaultParameterValue(null)>]?LatAxis         : Axis.LinearAxis,
            [<Optional;DefaultParameterValue(null)>]?LonAxis         : Axis.LinearAxis
        ) =
            (fun (ch:GenericChart) ->
                
                let map = 
                    Geo.init(
                        ?FitBounds      = FitBounds     ,
                        ?Resolution     = Resolution    ,
                        ?Scope          = Scope         ,
                        ?Projection     = Projection    ,
                        ?Center         = Center        ,
                        ?Visible        = Visible       ,
                        ?Domain         = Domain        ,
                        ?ShowCoastLines = ShowCoastLines,
                        ?CoastLineColor = CoastLineColor,
                        ?CoastLineWidth = CoastLineWidth,
                        ?ShowLand       = ShowLand      ,
                        ?LandColor      = LandColor     ,
                        ?ShowOcean      = ShowOcean     ,
                        ?OceanColor     = OceanColor    ,
                        ?ShowLakes      = ShowLakes     ,
                        ?LakeColor      = LakeColor     ,
                        ?ShowRivers     = ShowRivers    ,
                        ?RiverColor     = RiverColor    ,
                        ?RiverWidth     = RiverWidth    ,
                        ?ShowCountries  = ShowCountries ,
                        ?CountryColor   = CountryColor  ,
                        ?CountryWidth   = CountryWidth  ,
                        ?ShowSubunits   = ShowSubunits  ,
                        ?SubunitColor   = SubunitColor  ,
                        ?SubunitWidth   = SubunitWidth  ,
                        ?ShowFrame      = ShowFrame     ,
                        ?FrameColor     = FrameColor    ,
                        ?FrameWidth     = FrameWidth    ,
                        ?BgColor        = BgColor       ,
                        ?LatAxis        = LatAxis       ,
                        ?LonAxis        = LonAxis       
                    )
                let id = defaultArg Id 1
                ch |> Chart.withMap(map,id)
            )

        [<CompiledName("WithMapProjection")>]
        static member withMapProjection(projectionType : StyleParam.GeoProjectionType,
             [<Optional;DefaultParameterValue(null)>]?Rotation ,
             [<Optional;DefaultParameterValue(null)>]?Parallels,
             [<Optional;DefaultParameterValue(null)>]?Scale    ,
             [<Optional;DefaultParameterValue(null)>]?Id
            ) =
            (fun (ch:GenericChart) ->

                let projection = 
                    GeoProjection.init(
                        projectionType  = projectionType,
                        ?Rotation       = Rotation      ,
                        ?Parallels      = Parallels     ,
                        ?Scale          = Scale        
                    )

                let map = Geo.init(Projection     = projection)
                let id = defaultArg Id 1
                ch |> Chart.withMap(map,id)
            )

        /// <summary>Set the LayoutGrid options of a Chart</summary>
        /// <param name ="Rows">The number of rows in the grid. If you provide a 2D `subplots` array or a `yaxes` array, its length is used as the default. But it's also possible to have a different length, if you want to leave a row at the end for non-cartesian subplots.</param>
        /// <param name ="Columns">The number of columns in the grid. If you provide a 2D `subplots` array, the length of its longest row is used as the default. If you give an `xaxes` array, its length is used as the default. But it's also possible to have a different length, if you want to leave a row at the end for non-cartesian subplots.</param>
        /// <param name ="SubPlots">Used for freeform grids, where some axes may be shared across subplots but others are not. Each entry should be a cartesian subplot id, like "xy" or "x3y2", or "" to leave that cell empty. You may reuse x axes within the same column, and y axes within the same row. Non-cartesian subplots and traces that support `domain` can place themselves in this grid separately using the `gridcell` attribute.</param>
        /// <param name ="XAxes">Used with `yaxes` when the x and y axes are shared across columns and rows. Each entry should be an y axis id like "y", "y2", etc., or "" to not put a y axis in that row. Entries other than "" must be unique. Ignored if `subplots` is present. If missing but `xaxes` is present, will generate consecutive IDs.</param>
        /// <param name ="YAxes">Used with `yaxes` when the x and y axes are shared across columns and rows. Each entry should be an x axis id like "x", "x2", etc., or "" to not put an x axis in that column. Entries other than "" must be unique. Ignored if `subplots` is present. If missing but `yaxes` is present, will generate consecutive IDs.</param>
        /// <param name ="RowOrder">Is the first row the top or the bottom? Note that columns are always enumerated from left to right.</param>
        /// <param name ="Pattern">If no `subplots`, `xaxes`, or `yaxes` are given but we do have `rows` and `columns`, we can generate defaults using consecutive axis IDs, in two ways: "coupled" gives one x axis per column and one y axis per row. "independent" uses a new xy pair for each cell, left-to-right across each row then iterating rows according to `roworder`.</param>
        /// <param name ="XGap">Horizontal space between grid cells, expressed as a fraction of the total width available to one cell. Defaults to 0.1 for coupled-axes grids and 0.2 for independent grids.</param>
        /// <param name ="YGap">Vertical space between grid cells, expressed as a fraction of the total height available to one cell. Defaults to 0.1 for coupled-axes grids and 0.3 for independent grids.</param>
        /// <param name ="Domain">Sets the domains of this grid subplot (in plot fraction). The first and last cells end exactly at the domain edges, with no grout around the edges.</param>
        /// <param name ="XSide">Sets where the x axis labels and titles go. "bottom" means the very bottom of the grid. "bottom plot" is the lowest plot that each x axis is used in. "top" and "top plot" are similar.</param>
        /// <param name ="YSide">Sets where the y axis labels and titles go. "left" means the very left edge of the grid. "left plot" is the leftmost plot that each y axis is used in. "right" and "right plot" are similar.</param>
        [<CompiledName("WithLayoutGridStyle")>]
        static member withLayoutGridStyle([<Optional;DefaultParameterValue(null)>]?SubPlots   : (StyleParam.AxisId * StyleParam.AxisId) [] [],
            [<Optional;DefaultParameterValue(null)>]?XAxes      : StyleParam.AxisId [],
            [<Optional;DefaultParameterValue(null)>]?YAxes      : StyleParam.AxisId [],
            [<Optional;DefaultParameterValue(null)>]?Rows       : int,
            [<Optional;DefaultParameterValue(null)>]?Columns    : int,
            [<Optional;DefaultParameterValue(null)>]?RowOrder   : StyleParam.LayoutGridRowOrder,
            [<Optional;DefaultParameterValue(null)>]?Pattern    : StyleParam.LayoutGridPattern,
            [<Optional;DefaultParameterValue(null)>]?XGap       : float,
            [<Optional;DefaultParameterValue(null)>]?YGap       : float,
            [<Optional;DefaultParameterValue(null)>]?Domain     : Domain,
            [<Optional;DefaultParameterValue(null)>]?XSide      : StyleParam.LayoutGridXSide,
            [<Optional;DefaultParameterValue(null)>]?YSide      : StyleParam.LayoutGridYSide
        ) =
            (fun (ch:GenericChart) -> 
                let layout = GenericChart.getLayout ch
                let updatedGrid =   
                    let currentGrid = 
                        match layout.TryGetTypedValue<LayoutGrid> "grid" with
                        | Some grid -> grid
                        | None -> LayoutGrid()
                    currentGrid
                    |> LayoutGrid.style(
                        ?SubPlots    = SubPlots,
                        ?XAxes       = XAxes   ,
                        ?YAxes       = YAxes   ,
                        ?Rows        = Rows    ,
                        ?Columns     = Columns ,
                        ?RowOrder    = RowOrder,
                        ?Pattern     = Pattern ,
                        ?XGap        = XGap    ,
                        ?YGap        = YGap    ,
                        ?Domain      = Domain  ,
                        ?XSide       = XSide   ,
                        ?YSide       = YSide   
                    )
                let updatedLayout = layout |> Layout.SetLayoutGrid updatedGrid
                GenericChart.setLayout updatedLayout ch) 

        [<CompiledName("WithConfig")>]
        static member withConfig (config:Config) =
            (fun (ch:GenericChart) ->
                GenericChart.setConfig config ch)
        
        [<CompiledName("WithAnnotations")>]
        static member withAnnotations(annotations:seq<Annotation>) =
            (fun (ch:GenericChart) -> 
                ch
                |> GenericChart.mapLayout 
                    (Layout.style (Annotations = annotations)))

        // Set the title of a Chart
        [<CompiledName("WithTitle")>]
        static member withTitle(title,[<Optional;DefaultParameterValue(null)>] ?Titlefont) =
            (fun (ch:GenericChart) ->
                let layout =
                    Layout() 
                    |> Layout.style(Title=title,?Titlefont=Titlefont)
                GenericChart.addLayout layout ch
             )  


        // Set showLegend of a Chart
        [<CompiledName("WithLegend")>]
        static member withLegend(showlegend) =
            (fun (ch:GenericChart) ->
                let layout =
                    Layout() 
                    |> Layout.style(Showlegend=showlegend)
                GenericChart.addLayout layout ch
             )  

        
        // Set the size of a Chart
        [<CompiledName("WithSize")>]
        static member withSize(width,height) =
            (fun (ch:GenericChart) -> 
                let layout = 
                    GenericChart.getLayout ch
                    |> Layout.style (Width=width,Height=height)
                GenericChart.setLayout layout ch
            )

        // Set the margin of a Chart
        [<CompiledName("WithMargin")>]
        static member withMargin(margin:Margin) =
            (fun (ch:GenericChart) ->
                let layout =
                    GenericChart.getLayout ch 
                    |> Layout.style (Margin=margin)
                GenericChart.setLayout layout ch)   

        // Set the margin of a Chart
        [<CompiledName("WithMarginSize")>]
        static member withMarginSize
            (
                [<Optional;DefaultParameterValue(null)>] ?Left,
                [<Optional;DefaultParameterValue(null)>] ?Right,
                [<Optional;DefaultParameterValue(null)>] ?Top,
                [<Optional;DefaultParameterValue(null)>] ?Bottom,
                [<Optional;DefaultParameterValue(null)>] ?Pad,
                [<Optional;DefaultParameterValue(null)>] ?Autoexpand
            ) =                       
                let margin = 
                    Margin.init ( ?Left=Left,?Right=Right,?Top=Top,?Bottom=Bottom,?Pad=Pad,?Autoexpand=Autoexpand )
                Chart.withMargin(margin)

        [<CompiledName("WithTemplate")>]
        static member withTemplate(template: Template) =
            (fun (ch:GenericChart) ->
                ch
                |> GenericChart.mapLayout (fun l ->
                    template |> DynObj.setValue l "template"
                    l
                    )
            )

        // TODO: Include withLegend & withLegendStyle

    //Specifies the shape type to be drawn. If "line", a line is drawn from (`x0`,`y0`) to (`x1`,`y1`) If "circle", a circle is drawn from 
    //((`x0`+`x1`)/2, (`y0`+`y1`)/2)) with radius (|(`x0`+`x1`)/2 - `x0`|, |(`y0`+`y1`)/2 -`y0`)|) If "rect", a rectangle is drawn linking 
    //(`x0`,`y0`), (`x1`,`y0`), (`x1`,`y1`), (`x0`,`y1`), (`x0`,`y0`)  
        [<CompiledName("WithShape")>]
        static member withShape(shape:Shape) =
            (fun (ch:GenericChart) ->
                let layout = 
                    GenericChart.getLayout ch
                    |> Layout.style (Shapes=[shape])
                GenericChart.setLayout layout ch)


        [<CompiledName("WithShapes")>]
        static member withShapes(shapes:Shape seq) =
            (fun (ch:GenericChart) ->
                let layout = 
                    GenericChart.getLayout ch
                    |> Layout.style (Shapes=shapes)
                GenericChart.setLayout layout ch)


        // ####################### 
        /// Create a combined chart with the given charts merged
        [<CompiledName("Combine")>]
        static member Combine(gCharts:seq<GenericChart>) =
            GenericChart.combine gCharts

        /// <summary>
        /// Creates a subplot grid with the given dimensions (nRows x nCols) for the input charts.
        /// </summary>
        /// <param name ="nRows">The number of rows in the grid. If you provide a 2D `subplots` array or a `yaxes` array, its length is used as the default. But it's also possible to have a different length, if you want to leave a row at the end for non-cartesian subplots.</param>
        /// <param name ="nCols">The number of columns in the grid. If you provide a 2D `subplots` array, the length of its longest row is used as the default. If you give an `xaxes` array, its length is used as the default. But it's also possible to have a different length, if you want to leave a row at the end for non-cartesian subplots.</param>
        /// <param name ="SubPlots">Used for freeform grids, where some axes may be shared across subplots but others are not. Each entry should be a cartesian subplot id, like "xy" or "x3y2", or "" to leave that cell empty. You may reuse x axes within the same column, and y axes within the same row. Non-cartesian subplots and traces that support `domain` can place themselves in this grid separately using the `gridcell` attribute.</param>
        /// <param name ="XAxes">Used with `yaxes` when the x and y axes are shared across columns and rows. Each entry should be an y axis id like "y", "y2", etc., or "" to not put a y axis in that row. Entries other than "" must be unique. Ignored if `subplots` is present. If missing but `xaxes` is present, will generate consecutive IDs.</param>
        /// <param name ="YAxes">Used with `yaxes` when the x and y axes are shared across columns and rows. Each entry should be an x axis id like "x", "x2", etc., or "" to not put an x axis in that column. Entries other than "" must be unique. Ignored if `subplots` is present. If missing but `yaxes` is present, will generate consecutive IDs.</param>
        /// <param name ="RowOrder">Is the first row the top or the bottom? Note that columns are always enumerated from left to right.</param>
        /// <param name ="Pattern">If no `subplots`, `xaxes`, or `yaxes` are given but we do have `rows` and `columns`, we can generate defaults using consecutive axis IDs, in two ways: "coupled" gives one x axis per column and one y axis per row. "independent" uses a new xy pair for each cell, left-to-right across each row then iterating rows according to `roworder`.</param>
        /// <param name ="XGap">Horizontal space between grid cells, expressed as a fraction of the total width available to one cell. Defaults to 0.1 for coupled-axes grids and 0.2 for independent grids.</param>
        /// <param name ="YGap">Vertical space between grid cells, expressed as a fraction of the total height available to one cell. Defaults to 0.1 for coupled-axes grids and 0.3 for independent grids.</param>
        /// <param name ="Domain">Sets the domains of this grid subplot (in plot fraction). The first and last cells end exactly at the domain edges, with no grout around the edges.</param>
        /// <param name ="XSide">Sets where the x axis labels and titles go. "bottom" means the very bottom of the grid. "bottom plot" is the lowest plot that each x axis is used in. "top" and "top plot" are similar.</param>
        /// <param name ="YSide">Sets where the y axis labels and titles go. "left" means the very left edge of the grid. "left plot" is the leftmost plot that each y axis is used in. "right" and "right plot" are similar.</param>
        [<CompiledName("Grid")>]
        static member Grid (nRows: int, nCols: int,
            [<Optional;DefaultParameterValue(null)>]?SubPlots   : (StyleParam.AxisId*StyleParam.AxisId) [] [],
            [<Optional;DefaultParameterValue(null)>]?XAxes      : StyleParam.AxisId [],
            [<Optional;DefaultParameterValue(null)>]?YAxes      : StyleParam.AxisId [],
            [<Optional;DefaultParameterValue(null)>]?RowOrder   : StyleParam.LayoutGridRowOrder,
            [<Optional;DefaultParameterValue(null)>]?Pattern    : StyleParam.LayoutGridPattern,
            [<Optional;DefaultParameterValue(null)>]?XGap       : float,
            [<Optional;DefaultParameterValue(null)>]?YGap       : float,
            [<Optional;DefaultParameterValue(null)>]?Domain     : Domain,
            [<Optional;DefaultParameterValue(null)>]?XSide      : StyleParam.LayoutGridXSide,
            [<Optional;DefaultParameterValue(null)>]?YSide      : StyleParam.LayoutGridYSide
            ) =
                fun (gCharts:#seq<GenericChart>) ->
                    
                    let pattern = defaultArg Pattern StyleParam.LayoutGridPattern.Independent

                    let hasSharedAxes = pattern = StyleParam.LayoutGridPattern.Coupled

                    // rows x cols coordinate grid
                    let gridCoordinates = 
                        Array.init nRows (fun i ->
                            Array.init nCols (fun j ->
                                i+1,j+1
                            )
                        )
                        |> Array.concat

                    // extract all axes from the plots to later add them with an updated axis anchor
                    // TODO: currently only gets the default (first) x and y axis. There might be charts with multiple axes which might cause havoc downstream.
                    // those should either be removed or accounted for
                    let axes =
                        gCharts
                        |> Seq.map (fun gChart ->
                            gChart 
                            |> GenericChart.getLayout
                            |> fun l -> 
                                let xAxis = l.TryGetTypedValue<Axis.LinearAxis> "xaxis" |> Option.defaultValue (Axis.LinearAxis.init())
                                let yAxis = l.TryGetTypedValue<Axis.LinearAxis> "yaxis" |> Option.defaultValue (Axis.LinearAxis.init())
                                xAxis,yAxis
                        )

                    gCharts
                    |> Seq.zip gridCoordinates
                    |> Seq.zip axes
                    |> Seq.mapi (fun i ((xAxis,yAxis), ((y,x), gChart)) ->

                        let xAnchor, yAnchor = 
                            if hasSharedAxes then 
                                x, y //set axis anchors according to grid coordinates
                            else
                                i+1, i+1 //set individual axis anchors for each subplot

                        gChart
                        |> Chart.withAxisAnchor(xAnchor,yAnchor) // set adapted axis anchors
                        |> Chart.withX_Axis(xAxis,i+1) // set previous axis with adapted id (one individual axis for each subplot, wether or not they will be used later)
                        |> Chart.withY_Axis(yAxis,i+1) // set previous axis with adapted id (one individual axis for each subplot, wether or not they will be used later)
                        |> GenericChart.mapLayout (fun l ->
                            if i > 0 then 
                                // remove default axes from consecutive charts, otherwise they will override the first one
                                l.Remove("xaxis") |> ignore
                                l.Remove("yaxis") |> ignore
                            l
                        )
                    )
                    |> Chart.Combine
                    |> Chart.withLayoutGrid (
                        LayoutGrid.init(
                            Rows      = nRows,
                            Columns   = nCols,
                            Pattern   = pattern,
                            ?SubPlots = SubPlots,
                            ?XAxes    = XAxes,
                            ?YAxes    = YAxes,
                            ?RowOrder = RowOrder,
                            ?XGap     = XGap,
                            ?YGap     = YGap,
                            ?Domain   = Domain,
                            ?XSide    = XSide,
                            ?YSide    = YSide   
                        )
                    )

        /// <summary>
        /// Creates a subplot grid with the the dimensions of the input 2D sequence containing the charts to render in the respective cells.
        ///
        /// ATTENTION: when the individual rows do not have the same amount of charts, they will be filled with dummy charts TO THE RIGHT.
        ///
        /// prevent this behaviour by using Chart.Invisible at the cells that should be empty.
        /// </summary>
        /// <param name ="SubPlots">Used for freeform grids, where some axes may be shared across subplots but others are not. Each entry should be a cartesian subplot id, like "xy" or "x3y2", or "" to leave that cell empty. You may reuse x axes within the same column, and y axes within the same row. Non-cartesian subplots and traces that support `domain` can place themselves in this grid separately using the `gridcell` attribute.</param>
        /// <param name ="XAxes">Used with `yaxes` when the x and y axes are shared across columns and rows. Each entry should be an y axis id like "y", "y2", etc., or "" to not put a y axis in that row. Entries other than "" must be unique. Ignored if `subplots` is present. If missing but `xaxes` is present, will generate consecutive IDs.</param>
        /// <param name ="YAxes">Used with `yaxes` when the x and y axes are shared across columns and rows. Each entry should be an x axis id like "x", "x2", etc., or "" to not put an x axis in that column. Entries other than "" must be unique. Ignored if `subplots` is present. If missing but `yaxes` is present, will generate consecutive IDs.</param>
        /// <param name ="RowOrder">Is the first row the top or the bottom? Note that columns are always enumerated from left to right.</param>
        /// <param name ="Pattern">If no `subplots`, `xaxes`, or `yaxes` are given but we do have `rows` and `columns`, we can generate defaults using consecutive axis IDs, in two ways: "coupled" gives one x axis per column and one y axis per row. "independent" uses a new xy pair for each cell, left-to-right across each row then iterating rows according to `roworder`.</param>
        /// <param name ="XGap">Horizontal space between grid cells, expressed as a fraction of the total width available to one cell. Defaults to 0.1 for coupled-axes grids and 0.2 for independent grids.</param>
        /// <param name ="YGap">Vertical space between grid cells, expressed as a fraction of the total height available to one cell. Defaults to 0.1 for coupled-axes grids and 0.3 for independent grids.</param>
        /// <param name ="Domain">Sets the domains of this grid subplot (in plot fraction). The first and last cells end exactly at the domain edges, with no grout around the edges.</param>
        /// <param name ="XSide">Sets where the x axis labels and titles go. "bottom" means the very bottom of the grid. "bottom plot" is the lowest plot that each x axis is used in. "top" and "top plot" are similar.</param>
        /// <param name ="YSide">Sets where the y axis labels and titles go. "left" means the very left edge of the grid. "left plot" is the leftmost plot that each y axis is used in. "right" and "right plot" are similar.</param>
        [<CompiledName("Grid")>]
        static member Grid 
            (
                [<Optional;DefaultParameterValue(null)>]?SubPlots   : (StyleParam.AxisId*StyleParam.AxisId) [] [],
                [<Optional;DefaultParameterValue(null)>]?XAxes      : StyleParam.AxisId [],
                [<Optional;DefaultParameterValue(null)>]?YAxes      : StyleParam.AxisId [],
                [<Optional;DefaultParameterValue(null)>]?RowOrder   : StyleParam.LayoutGridRowOrder,
                [<Optional;DefaultParameterValue(null)>]?Pattern    : StyleParam.LayoutGridPattern,
                [<Optional;DefaultParameterValue(null)>]?XGap       : float,
                [<Optional;DefaultParameterValue(null)>]?YGap       : float,
                [<Optional;DefaultParameterValue(null)>]?Domain     : Domain,
                [<Optional;DefaultParameterValue(null)>]?XSide      : StyleParam.LayoutGridXSide,
                [<Optional;DefaultParameterValue(null)>]?YSide      : StyleParam.LayoutGridYSide
            ) =
                fun (gCharts:#seq<#seq<GenericChart>>) ->
            
                    let nRows = Seq.length gCharts
                    let nCols = Seq.maxBy Seq.length gCharts |> Seq.length

                    if Seq.exists (fun s -> (s |> Seq.length) <> nCols) gCharts then
                        printfn "WARNING: not all rows contain the same amount of charts."
                        printfn "The rows will be filled TO THE RIGHT with invisible dummy charts."
                        printfn "To have more positional control, use Chart.Empty() in your Grid where you want to have empty cells."

                        let copy = 
                            gCharts 
                            |> Seq.map Seq.cast<GenericChart.GenericChart> // this is ugly but i did not find another way for the inner seq to be be a flexible type (so you can use list, array, and seq).

                        let newGrid =
                            copy
                            |> Seq.map (fun (row) ->
                                let nCharts = Seq.length row
                                if nCharts <> nCols then
                                    seq {yield! row; for i in nCharts .. nCols-1 do yield Chart.Invisible()}
                                else
                                    row
                            )
                            |> Seq.concat

                        newGrid
                        |> Chart.Grid(
                            nRows,nCols,
                            ?SubPlots = SubPlots,
                            ?XAxes    = XAxes,
                            ?YAxes    = YAxes,
                            ?RowOrder = RowOrder,
                            ?Pattern  = Pattern,
                            ?XGap     = XGap,
                            ?YGap     = YGap,
                            ?Domain   = Domain,
                            ?XSide    = XSide,
                            ?YSide    = YSide   
                        )

                    else
                        gCharts
                        |> Seq.concat
                        |> Chart.Grid(
                            nRows,nCols,
                            ?SubPlots = SubPlots,
                            ?XAxes    = XAxes,
                            ?YAxes    = YAxes,
                            ?RowOrder = RowOrder,
                            ?Pattern  = Pattern,
                            ?XGap     = XGap,
                            ?YGap     = YGap,
                            ?Domain   = Domain,
                            ?XSide    = XSide,
                            ?YSide    = YSide   
                        )

        /// Creates a chart stack (a subplot grid with one column) from the input charts.
        /// </summary>
        /// <param name ="SubPlots">Used for freeform grids, where some axes may be shared across subplots but others are not. Each entry should be a cartesian subplot id, like "xy" or "x3y2", or "" to leave that cell empty. You may reuse x axes within the same column, and y axes within the same row. Non-cartesian subplots and traces that support `domain` can place themselves in this grid separately using the `gridcell` attribute.</param>
        /// <param name ="XAxes">Used with `yaxes` when the x and y axes are shared across columns and rows. Each entry should be an y axis id like "y", "y2", etc., or "" to not put a y axis in that row. Entries other than "" must be unique. Ignored if `subplots` is present. If missing but `xaxes` is present, will generate consecutive IDs.</param>
        /// <param name ="YAxes">Used with `yaxes` when the x and y axes are shared across columns and rows. Each entry should be an x axis id like "x", "x2", etc., or "" to not put an x axis in that column. Entries other than "" must be unique. Ignored if `subplots` is present. If missing but `yaxes` is present, will generate consecutive IDs.</param>
        /// <param name ="RowOrder">Is the first row the top or the bottom? Note that columns are always enumerated from left to right.</param>
        /// <param name ="Pattern">If no `subplots`, `xaxes`, or `yaxes` are given but we do have `rows` and `columns`, we can generate defaults using consecutive axis IDs, in two ways: "coupled" gives one x axis per column and one y axis per row. "independent" uses a new xy pair for each cell, left-to-right across each row then iterating rows according to `roworder`.</param>
        /// <param name ="XGap">Horizontal space between grid cells, expressed as a fraction of the total width available to one cell. Defaults to 0.1 for coupled-axes grids and 0.2 for independent grids.</param>
        /// <param name ="YGap">Vertical space between grid cells, expressed as a fraction of the total height available to one cell. Defaults to 0.1 for coupled-axes grids and 0.3 for independent grids.</param>
        /// <param name ="Domain">Sets the domains of this grid subplot (in plot fraction). The first and last cells end exactly at the domain edges, with no grout around the edges.</param>
        /// <param name ="XSide">Sets where the x axis labels and titles go. "bottom" means the very bottom of the grid. "bottom plot" is the lowest plot that each x axis is used in. "top" and "top plot" are similar.</param>
        /// <param name ="YSide">Sets where the y axis labels and titles go. "left" means the very left edge of the grid. "left plot" is the leftmost plot that each y axis is used in. "right" and "right plot" are similar.</param>
        [<CompiledName("SingleStack")>]
        static member SingleStack 
            (
                [<Optional;DefaultParameterValue(null)>]?SubPlots   : (StyleParam.AxisId*StyleParam.AxisId) [] [],
                [<Optional;DefaultParameterValue(null)>]?XAxes      : StyleParam.AxisId [],
                [<Optional;DefaultParameterValue(null)>]?YAxes      : StyleParam.AxisId [],
                [<Optional;DefaultParameterValue(null)>]?RowOrder   : StyleParam.LayoutGridRowOrder,
                [<Optional;DefaultParameterValue(null)>]?Pattern    : StyleParam.LayoutGridPattern,
                [<Optional;DefaultParameterValue(null)>]?XGap       : float,
                [<Optional;DefaultParameterValue(null)>]?YGap       : float,
                [<Optional;DefaultParameterValue(null)>]?Domain     : Domain,
                [<Optional;DefaultParameterValue(null)>]?XSide      : StyleParam.LayoutGridXSide,
                [<Optional;DefaultParameterValue(null)>]?YSide      : StyleParam.LayoutGridYSide
            ) = 
            
                fun (gCharts: #seq<GenericChart.GenericChart>) ->
                
                    gCharts
                    |> Chart.Grid(
                        nRows = Seq.length gCharts,
                        nCols = 1,
                        ?SubPlots = SubPlots,
                        ?XAxes    = XAxes,
                        ?YAxes    = YAxes,
                        ?RowOrder = RowOrder,
                        ?Pattern  = Pattern,
                        ?XGap     = XGap,
                        ?YGap     = YGap,
                        ?Domain   = Domain,
                        ?XSide    = XSide,
                        ?YSide    = YSide   
                    )

        /// Create a combined chart with the given charts merged
        [<Obsolete("Use Chart.Grid for multi column grid charts or SingleStack for one-column stacked charts.")>]
        [<CompiledName("Stack")>]
        static member Stack ( [<Optional;DefaultParameterValue(null)>] ?Columns:int, 
                [<Optional;DefaultParameterValue(null)>] ?Space) = 
            (fun (charts:#seq<GenericChart>) ->  

                let col = defaultArg Columns 2
                let len      = charts |> Seq.length
                let colWidth = 1. / float col
                let rowWidth = 
                    let tmp = float len / float col |> ceil
                    1. / tmp
                let space = 
                    let s = defaultArg Space 0.05
                    if s < 0. || s > 1. then 
                        printfn "Space should be between 0.0 - 1.0. Automaticaly set to default (0.05)"
                        0.05
                    else
                        s

                let contains3d ch =
                    ch 
                    |> existsTrace (fun t -> 
                        match t with
                        | :? Trace3d -> true
                        | _ -> false)

                charts
                |> Seq.mapi (fun i ch ->
                    let colI,rowI,index = (i%col+1), (i/col+1),(i+1)
                    let xdomain = (colWidth * float (colI-1), (colWidth * float colI) - space ) 
                    let ydomain = (1. - ((rowWidth * float rowI) - space ),1. - (rowWidth * float (rowI-1)))

                    if contains3d ch then
                        let sceneName = sprintf "scene%i" (i+1)

                        let scene =
                            Scene.init (
                                Domain =
                                    Domain.init(X = StyleParam.Range.MinMax xdomain,Y= StyleParam.Range.MinMax ydomain)
                            )
                        let layout = 
                            GenericChart.getLayout ch
                            |> Layout.AddScene (
                                    sceneName, 
                                    scene
                                )
                        ch
                        |> mapTrace 
                            (fun t -> 
                                t?scene <- sceneName
                                t
                            )
                        |> GenericChart.setLayout layout
                        //|> Chart.withAxisAnchor(X=index,Y=index) 
                    else

                        let xaxis,yaxis,layout = 
                            let layout = GenericChart.getLayout ch
                            let xName, yName = StyleParam.AxisId.X 1 |> StyleParam.AxisId.toString, StyleParam.AxisId.Y 1 |> StyleParam.AxisId.toString
                            match (layout.TryGetTypedValue<Axis.LinearAxis> xName),(layout.TryGetTypedValue<Axis.LinearAxis> yName) with
                            | Some x, Some y ->
                                // remove axis
                                DynObj.remove layout xName
                                DynObj.remove layout yName

                                x |> Axis.LinearAxis.style(Anchor=StyleParam.AxisAnchorId.Y index,Domain=StyleParam.Range.MinMax xdomain),
                                y |> Axis.LinearAxis.style(Anchor=StyleParam.AxisAnchorId.X index,Domain=StyleParam.Range.MinMax ydomain),
                                layout
                            | Some x, None -> 
                                // remove x - axis
                                DynObj.remove layout xName
                                x |> Axis.LinearAxis.style(Anchor=StyleParam.AxisAnchorId.Y index,Domain=StyleParam.Range.MinMax xdomain),
                                Axis.LinearAxis.init(Anchor=StyleParam.AxisAnchorId.X index,Domain=StyleParam.Range.MinMax ydomain),
                                layout
                            | None, Some y -> 
                                // remove y - axis
                                DynObj.remove layout yName
                                Axis.LinearAxis.init(Anchor=StyleParam.AxisAnchorId.Y index,Domain=StyleParam.Range.MinMax xdomain),
                                y |> Axis.LinearAxis.style(Anchor=StyleParam.AxisAnchorId.X index,Domain=StyleParam.Range.MinMax ydomain),
                                layout
                            | None, None ->
                                Axis.LinearAxis.init(Anchor=StyleParam.AxisAnchorId.Y index,Domain=StyleParam.Range.MinMax xdomain),
                                Axis.LinearAxis.init(Anchor=StyleParam.AxisAnchorId.X index,Domain=StyleParam.Range.MinMax ydomain),
                                layout

                        ch
                        |> GenericChart.setLayout layout
                        |> Chart.withAxisAnchor(X=index,Y=index) 
                        |> Chart.withX_Axis(xaxis,index)
                        |> Chart.withY_Axis(yaxis,index)
                    )

                |> Chart.Combine
                )
        
// ############################################################
// ####################### Apply to DisplayOptions

        /// Show chart in browser
        [<CompiledName("WithDescription")>]
        static member WithDescription (description:ChartDescription) (ch:GenericChart) = 
            ch 
            |> mapDisplayOptions (DisplayOptions.style(Description=description))


        /// Adds the given additional html tags on the chart's DisplayOptions. They will be included in the document's <head>
        [<CompiledName("WithAdditionalHeadTags")>]
        static member WithAdditionalHeadTags (additionalHeadTags:seq<string>) (ch:GenericChart) = 
            ch 
            |> mapDisplayOptions ( fun d ->
                let tags = d.TryGetTypedValue<seq<string>>("AdditionalHeadTags")
                let newTags =
                    tags
                    |> Option.map (fun tags -> seq{yield! tags; yield! additionalHeadTags})
                    |> Option.defaultValue additionalHeadTags
                d |> DisplayOptions.style(AdditionalHeadTags=newTags)
            )

        /// Sets the given additional head tags on the chart's DisplayOptions. They will be included in the document's <head>
        [<CompiledName("WithHeadTags")>]
        static member WithHeadTags (headTags:seq<string>) (ch:GenericChart) = 
            ch 
            |> mapDisplayOptions (DisplayOptions.style(AdditionalHeadTags=headTags))

        
        /// Adds the necessary script tags to render tex strings to the chart's DisplayOptions
        [<CompiledName("WithMathTex")>]
        static member WithMathTex ([<Optional;DefaultParameterValue(true)>]?AppendTags:bool) = 
            let tags = [
                """<script type="text/x-mathjax-config;executed=true">MathJax.Hub.Config({tex2jax: {inlineMath: [['$','$'], ['\\(','\\)']], processEscapes: true}});</script>"""
                """<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.5/MathJax.js?config=TeX-AMS-MML_HTMLorMML%2CSafe.js&ver=4.1"></script>"""
            ]
            (fun (ch:GenericChart) ->
            
                if (AppendTags |> Option.defaultValue true) then
                    ch |> Chart.WithAdditionalHeadTags tags
                else
                    ch |> Chart.WithHeadTags tags
            )

        /// Save chart as html single page
        [<CompiledName("SaveHtmlAs")>]
        static member SaveHtmlAs pathName (ch:GenericChart,[<Optional;DefaultParameterValue(null)>] ?Verbose) =
            let html = GenericChart.toEmbeddedHTML ch
            let file = sprintf "%s.html" pathName // remove file extension
            File.WriteAllText(file, html)

            let verbose = defaultArg Verbose false
            if verbose then
                file |> openOsSpecificFile

        /// Show chart in browser
        [<CompiledName("Show")>]
        static member Show (ch:GenericChart) = 
            let guid = Guid.NewGuid().ToString()
            let html = GenericChart.toEmbeddedHTML ch
            let tempPath = Path.GetTempPath()
            let file = sprintf "%s.html" guid
            let path = Path.Combine(tempPath, file)
            File.WriteAllText(path, html)
            path |> openOsSpecificFile

        /// Show chart in browser
        [<CompiledName("ShowAsImage")>]
        static member ShowAsImage (format:StyleParam.ImageFormat) (ch:GenericChart) = 
            let guid = Guid.NewGuid().ToString()
            let html = GenericChart.toEmbeddedImage format ch
            let tempPath = Path.GetTempPath()
            let file = sprintf "%s.html" guid
            let path = Path.Combine(tempPath, file)
            File.WriteAllText(path, html)
            path |> openOsSpecificFile
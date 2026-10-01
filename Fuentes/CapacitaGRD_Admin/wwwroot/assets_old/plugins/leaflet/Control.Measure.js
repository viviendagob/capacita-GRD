L.Control.Measure = L.Control.extend({
    options: {
        position: 'bottomright'
    },

    onAdd: function(map) {
        var className = 'leaflet-control-zoom leaflet-bar leaflet-control',
            container = L.DomUtil.create('div', className);

        this._createButton('<img  src="data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iaXNvLTg4NTktMSI/Pg0KPCEtLSBHZW5lcmF0b3I6IEFkb2JlIElsbHVzdHJhdG9yIDE5LjAuMCwgU1ZHIEV4cG9ydCBQbHVnLUluIC4gU1ZHIFZlcnNpb246IDYuMDAgQnVpbGQgMCkgIC0tPg0KPHN2ZyB2ZXJzaW9uPSIxLjEiIGlkPSJDYXBhXzEiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiIHg9IjBweCIgeT0iMHB4Ig0KCSB2aWV3Qm94PSIwIDAgMzMgMzMiIHN0eWxlPSJlbmFibGUtYmFja2dyb3VuZDpuZXcgMCAwIDMzIDMzOyIgeG1sOnNwYWNlPSJwcmVzZXJ2ZSI+DQo8Zz4NCgk8cGF0aCBkPSJNMTAuNTc3LDMyLjk5OGMtMC4xMzMsMC0wLjI2LTAuMDUzLTAuMzU0LTAuMTQ2TDAuMTQ2LDIyLjc3M2MtMC4xOTUtMC4xOTUtMC4xOTUtMC41MTIsMC0wLjcwN0wyMi4wNywwLjE0Mw0KCQljMC4xODgtMC4xODgsMC41Mi0wLjE4OCwwLjcwNywwbDEwLjA3NiwxMC4wNzhjMC4xOTUsMC4xOTUsMC4xOTUsMC41MTIsMCwwLjcwN0wxMC45MywzMi44NTINCgkJQzEwLjgzNiwzMi45NDUsMTAuNzA5LDMyLjk5OCwxMC41NzcsMzIuOTk4eiBNMS4yMDcsMjIuNDJsOS4zNyw5LjM3MWwyMS4yMTYtMjEuMjE3bC05LjM2OS05LjM3MUwxLjIwNywyMi40MnoiLz4NCgk8Zz4NCgkJPHBhdGggZD0iTTIxLjE0NCw3LjIwN2MtMC4xMjgsMC0wLjI1Ni0wLjA0OS0wLjM1NC0wLjE0NmwtMi40NjctMi40NjdjLTAuMTk1LTAuMTk1LTAuMTk1LTAuNTEyLDAtMC43MDdzMC41MTItMC4xOTUsMC43MDcsMA0KCQkJbDIuNDY3LDIuNDY3YzAuMTk1LDAuMTk1LDAuMTk1LDAuNTEyLDAsMC43MDdDMjEuNCw3LjE1OCwyMS4yNzIsNy4yMDcsMjEuMTQ0LDcuMjA3eiIvPg0KCQk8cGF0aCBkPSJNMTkuMTE5LDEyLjM5NmMtMC4xMjgsMC0wLjI1Ni0wLjA0OS0wLjM1NC0wLjE0NmwtNC4wNDktNC4wNDljLTAuMTk1LTAuMTk1LTAuMTk1LTAuNTEyLDAtMC43MDdzMC41MTItMC4xOTUsMC43MDcsMA0KCQkJbDQuMDQ5LDQuMDQ5YzAuMTk1LDAuMTk1LDAuMTk1LDAuNTEyLDAsMC43MDdDMTkuMzc1LDEyLjM0OCwxOS4yNDcsMTIuMzk2LDE5LjExOSwxMi4zOTZ6Ii8+DQoJCTxwYXRoIGQ9Ik0xMy45MjgsMTQuNDI0Yy0wLjEyOCwwLTAuMjU2LTAuMDQ5LTAuMzU0LTAuMTQ2bC0yLjQ2Ny0yLjQ2N2MtMC4xOTUtMC4xOTUtMC4xOTUtMC41MTIsMC0wLjcwN3MwLjUxMi0wLjE5NSwwLjcwNywwDQoJCQlsMi40NjcsMi40NjdjMC4xOTUsMC4xOTUsMC4xOTUsMC41MTIsMCwwLjcwN0MxNC4xODQsMTQuMzc1LDE0LjA1NiwxNC40MjQsMTMuOTI4LDE0LjQyNHoiLz4NCgkJPHBhdGggZD0iTTExLjkwMSwxOS42MTVjLTAuMTI4LDAtMC4yNTYtMC4wNDktMC4zNTQtMC4xNDZMNy40OTgsMTUuNDJjLTAuMTk1LTAuMTk1LTAuMTk1LTAuNTEyLDAtMC43MDdzMC41MTItMC4xOTUsMC43MDcsMA0KCQkJbDQuMDQ5LDQuMDQ5YzAuMTk1LDAuMTk1LDAuMTk1LDAuNTEyLDAsMC43MDdDMTIuMTU3LDE5LjU2NiwxMi4wMjksMTkuNjE1LDExLjkwMSwxOS42MTV6Ii8+DQoJCTxwYXRoIGQ9Ik02LjcxMSwyMS42NGMtMC4xMjgsMC0wLjI1Ni0wLjA0OS0wLjM1NC0wLjE0NmwtMi40NjctMi40NjdjLTAuMTk1LTAuMTk1LTAuMTk1LTAuNTEyLDAtMC43MDdzMC41MTItMC4xOTUsMC43MDcsMA0KCQkJbDIuNDY3LDIuNDY3YzAuMTk1LDAuMTk1LDAuMTk1LDAuNTEyLDAsMC43MDdDNi45NjcsMjEuNTkxLDYuODM5LDIxLjY0LDYuNzExLDIxLjY0eiIvPg0KCTwvZz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjwvc3ZnPg0K">', 'Measure', 'leaflet-control-measure leaflet-bar-part leaflet-bar-part-top-and-bottom', container, this._toggleMeasure, this);

        return container;
    },

    _createButton: function(html, title, className, container, fn, context) {
        var link = L.DomUtil.create('a', className, container);
        link.innerHTML = html;
        link.href = '#';
        link.title = title;

        L.DomEvent
            .on(link, 'click', L.DomEvent.stopPropagation)
            .on(link, 'click', L.DomEvent.preventDefault)
            .on(link, 'click', fn, context)
            .on(link, 'dblclick', L.DomEvent.stopPropagation);

        return link;
    },

    _toggleMeasure: function() {
        this._measuring = !this._measuring;

        if (this._measuring) {
            L.DomUtil.addClass(this._container, 'leaflet-control-measure-on');
            this._startMeasuring();
        } else {
            L.DomUtil.removeClass(this._container, 'leaflet-control-measure-on');
            this._stopMeasuring();
        }
    },

    _startMeasuring: function() {
        this._oldCursor = this._map._container.style.cursor;
        this._map._container.style.cursor = 'crosshair';

        this._doubleClickZoom = this._map.doubleClickZoom.enabled();
        this._map.doubleClickZoom.disable();

        L.DomEvent
            .on(this._map, 'mousemove', this._mouseMove, this)
            .on(this._map, 'click', this._mouseClick, this)
            .on(this._map, 'dblclick', this._finishPath, this)
            .on(document, 'keydown', this._onKeyDown, this);

        if (!this._layerPaint) {
            this._layerPaint = L.layerGroup().addTo(this._map);
        }

        if (!this._points) {
            this._points = [];
        }
    },

    _stopMeasuring: function() {
        this._map._container.style.cursor = this._oldCursor;

        L.DomEvent
            .off(document, 'keydown', this._onKeyDown, this)
            .off(this._map, 'mousemove', this._mouseMove, this)
            .off(this._map, 'click', this._mouseClick, this)
            .off(this._map, 'dblclick', this._mouseClick, this);

        if (this._doubleClickZoom) {
            this._map.doubleClickZoom.enable();
        }

        if (this._layerPaint) {
            this._layerPaint.clearLayers();
        }

        this._restartPath();
    },

    _mouseMove: function(e) {
        if (!e.latlng || !this._lastPoint) {
            return;
        }

        if (!this._layerPaintPathTemp) {
            this._layerPaintPathTemp = L.polyline([this._lastPoint, e.latlng], {
                color: '#66D9EF',
                weight: 1.5,
                clickable: false,
                dashArray: '6,3'
            }).addTo(this._layerPaint);
        } else {
            //this._layerPaintPathTemp.spliceLatLngs(0, 2, this._lastPoint, e.latlng);
        }

        if (this._tooltip) {
            if (!this._distance) {
                this._distance = 0;
            }

            this._updateTooltipPosition(e.latlng);

            var distance = e.latlng.distanceTo(this._lastPoint);
            this._updateTooltipDistance(this._distance + distance, distance);
        }
    },

    _mouseClick: function(e) {
        // Skip if no coordinates
        if (!e.latlng) {
            return;
        }

        // If we have a tooltip, update the distance and create a new tooltip, leaving the old one exactly where it is (i.e. where the user has clicked)
        if (this._lastPoint && this._tooltip) {
            if (!this._distance) {
                this._distance = 0;
            }

            this._updateTooltipPosition(e.latlng);

            var distance = e.latlng.distanceTo(this._lastPoint);
            this._updateTooltipDistance(this._distance + distance, distance);

            this._distance += distance;
        }
        this._createTooltip(e.latlng);


        // If this is already the second click, add the location to the fix path (create one first if we don't have one)
        if (this._lastPoint && !this._layerPaintPath) {
            this._layerPaintPath = L.polyline([this._lastPoint], {
                color: '#FFFFFF',
                weight: 2,
                clickable: false
            }).addTo(this._layerPaint);
        }

        if (this._layerPaintPath) {
            this._layerPaintPath.addLatLng(e.latlng);
        }

        // Upate the end marker to the current location
        if (this._lastCircle) {
            this._layerPaint.removeLayer(this._lastCircle);
        }

        this._lastCircle = new L.CircleMarker(e.latlng, {
            color: '#FFFFFF',
            opacity: 1,
            weight: 1,
            fill: true,
            fillOpacity: 1,
            radius: 2,
            clickable: this._lastCircle ? true : false
        }).addTo(this._layerPaint);

        this._lastCircle.on('click', function() { this._finishPath(); }, this);

        // Save current location as last location
        this._lastPoint = e.latlng;
    },

    _finishPath: function() {
        // Remove the last end marker as well as the last (moving tooltip)
        if (this._lastCircle) {
            this._layerPaint.removeLayer(this._lastCircle);
        }
        if (this._tooltip) {
            this._layerPaint.removeLayer(this._tooltip);
        }
        if (this._layerPaint && this._layerPaintPathTemp) {
            this._layerPaint.removeLayer(this._layerPaintPathTemp);
        }

        // Reset everything
        this._restartPath();
    },

    _restartPath: function() {
        this._distance = 0;
        this._tooltip = undefined;
        this._lastCircle = undefined;
        this._lastPoint = undefined;
        this._layerPaintPath = undefined;
        this._layerPaintPathTemp = undefined;
    },

    _createTooltip: function(position) {
        var icon = L.divIcon({
            className: 'leaflet-measure-tooltip',
            iconAnchor: [-5, -5]
        });
        this._tooltip = L.marker(position, {
            icon: icon,
            clickable: false
        }).addTo(this._layerPaint);
    },

    _updateTooltipPosition: function(position) {
        this._tooltip.setLatLng(position);
    },

    _updateTooltipDistance: function(total, difference) {
        var totalRound = this._round(total),
            differenceRound = this._round(difference);

        var text = '<div class="leaflet-measure-tooltip-total">' + new Intl.NumberFormat("es-PE").format(totalRound) + ' Km</div>';
        if (differenceRound > 0 && totalRound != differenceRound) {
            text += '<div class="leaflet-measure-tooltip-difference">(+' + new Intl.NumberFormat("es-PE").format(differenceRound) + ' Km)</div>';
        }

        this._tooltip._icon.innerHTML = text;
    },

    _round: function(val) {

        return (parseFloat(val) / 1000).toFixed(2);
        //return Math.round((val / 1852) * 10) / 10;
    },

    _onKeyDown: function(e) {
        if (e.keyCode == 27) {
            // If not in path exit measuring mode, else just finish path
            if (!this._lastPoint) {
                this._toggleMeasure();
            } else {
                this._finishPath();
            }
        }
    }
});

L.Map.mergeOptions({
    measureControl: false
});

L.Map.addInitHook(function() {
    if (this.options.measureControl) {
        this.measureControl = new L.Control.Measure();
        this.addControl(this.measureControl);
    }
});

L.control.measure = function(options) {
    return new L.Control.Measure(options);
};
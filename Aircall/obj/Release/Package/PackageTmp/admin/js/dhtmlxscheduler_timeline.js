/*
@license
dhtmlxScheduler v.4.3.25 Professional

This software can be used only as part of dhtmlx.com site.
You are not allowed to use it on any other site

(c) Dinamenta, UAB.


*/
Scheduler.plugin(function (e) {
    e._temp_matrix_scope = function () {
        function t() {
            for (var t = e.get_visible_events(), a = [], n = 0; n < this.y_unit.length; n++) a[n] = [];
            var i;
            a[i] || (a[i] = []);
            for (var n = 0; n < t.length; n++) {
                i = this.order[t[n][this.y_property]];
                for (var r = 0; this._trace_x[r + 1] && t[n].start_date >= this._trace_x[r + 1];) r++;
                for (; this._trace_x[r] && t[n].end_date > this._trace_x[r];) a[i][r] || (a[i][r] = []), a[i][r].push(t[n]), r++
            }
            return a
        }

        function a(t, a, n) {
            var i = 0,
                r = n._step,
                d = n.round_position,
                l = 0,
                o = a ? t.end_date : t.start_date;
            o.valueOf() > e._max_date.valueOf() && (o = e._max_date);
            var s = o - e._min_date_timeline;
            if (s > 0) {
                var _ = e._get_date_index(n, o);
                e._ignores[_] && (d = !0);
                for (var c = 0; _ > c; c++) i += e._cols[c];
                var u = e.date.add(e._min_date_timeline, e.matrix[e._mode].x_step * _, e.matrix[e._mode].x_unit);
                d ? +o > +u && a && (l = e._cols[_]) : (s = o - u, n.first_hour || n.last_hour ? (s -= n._start_correction, 0 > s && (s = 0), l = Math.round(s / r), l > e._cols[_] && (l = e._cols[_])) : l = Math.round(s / r))
            }
            return i += a ? 0 === s || d ? l - 14 : l - 12 : l + 1
        }

        function n(t, a) {
            var n = e._get_date_index(this, t),
                i = this._trace_x[n];
            return a && +t != +this._trace_x[n] && (i = this._trace_x[n + 1] ? this._trace_x[n + 1] : e.date.add(this._trace_x[n], this.x_step, this.x_unit)), new Date(i)
        }

        function i(t) {
            if (e._ignores_detected)
                for (var a, n, i, r, d = 0; d < t.length; d++) {
                    for (r = t[d], i = !1, a = e._get_date_index(this, r.start_date), n = e._get_date_index(this, r.end_date) ; n > a;) {
                        if (!e._ignores[a]) {
                            i = !0;
                            break
                        }
                        a++
                    }
                    i || a != n || e._ignores[n] || +r.end_date > +this._trace_x[n] && (i = !0), i || (t.splice(d, 1), d--)
                }
        }

        function r(t) {
            var a = "";
            if (t && "cell" != this.render) {
                i.call(this, t), t.sort(this.sort || function (e, t) {
                    return e.start_date.valueOf() == t.start_date.valueOf() ? e.id > t.id ? 1 : -1 : e.start_date > t.start_date ? 1 : -1
                });
                for (var r = [], d = t.length, l = 0; d > l; l++) {
                    var o = t[l];
                    o._inner = !1;
                    var s = this.round_position ? n.apply(this, [o.start_date, !1]) : o.start_date;
                    for (this.round_position ? n.apply(this, [o.end_date, !0]) : o.end_date; r.length;) {
                        var _ = r[r.length - 1];
                        if (!(_.end_date.valueOf() <= s.valueOf())) break;
                        r.splice(r.length - 1, 1)
                    }
                    for (var c = !1, u = 0; u < r.length; u++) {
                        var h = r[u];
                        if (h.end_date.valueOf() <= s.valueOf()) {
                            c = !0, o._sorder = h._sorder,
                                r.splice(u, 1), o._inner = !0;
                            break
                        }
                    }
                    if (r.length && (r[r.length - 1]._inner = !0), !c)
                        if (r.length)
                            if (r.length <= r[r.length - 1]._sorder) {
                                if (r[r.length - 1]._sorder)
                                    for (var p = 0; p < r.length; p++) {
                                        for (var v = !1, m = 0; m < r.length; m++)
                                            if (r[m]._sorder == p) {
                                                v = !0;
                                                break
                                            }
                                        if (!v) {
                                            o._sorder = p;
                                            break
                                        }
                                    } else o._sorder = 0;
                                o._inner = !0
                            } else {
                                for (var g = r[0]._sorder, f = 1; f < r.length; f++) r[f]._sorder > g && (g = r[f]._sorder);
                                o._sorder = g + 1, o._inner = !1
                            }
                        else o._sorder = 0;
                    r.push(o), r.length > (r.max_count || 0) ? (r.max_count = r.length, o._count = r.length) : o._count = o._count ? o._count : 1;
                }
                for (var b = 0; b < t.length; b++) t[b]._count = r.max_count;
                for (var y = 0; d > y; y++) a += e.render_timeline_event.call(this, t[y], !1)
            }
            return a
        }

        function d(a) {
            var n = "<table style='table-layout:fixed;' cellspacing='0' cellpadding='0'>",
                i = [];
            if (e._load_mode && e._load(), "cell" == this.render) i = t.call(this);
            else
                for (var d = e.get_visible_events(), l = this.order, o = 0; o < d.length; o++) {
                    var s = d[o],
                        _ = s[this.y_property],
                        c = this.order[_];
                    if (this.show_unassigned && !_) {
                        for (var u in l)
                            if (l.hasOwnProperty(u)) {
                                c = l[u], i[c] || (i[c] = []);
                                var h = e._lame_copy({}, s);
                                h[this.y_property] = u, i[c].push(h)
                            }
                    } else i[c] || (i[c] = []), i[c].push(s)
                }
            for (var p = 0, v = 0; v < e._cols.length; v++) p += e._cols[v];
            var m = new Date,
                g = e._cols.length - e._ignores_detected;
            m = (e.date.add(m, this.x_step * g, this.x_unit) - m - (this._start_correction + this._end_correction) * g) / p, this._step = m, this._summ = p;
            var f = e._colsS.heights = [],
                b = [];
            this._events_height = {}, this._section_height = {};
            for (var v = 0; v < this.y_unit.length; v++) {
                var y = this._logic(this.render, this.y_unit[v], this);
                e._merge(y, {
                    height: this.dy
                }), this.section_autoheight && (this.y_unit.length * y.height < a.offsetHeight && (y.height = Math.max(y.height, Math.floor((a.offsetHeight - 1) / this.y_unit.length))),
                    this._section_height[this.y_unit[v].key] = y.height), y.td_className || (y.td_className = "dhx_matrix_scell" + (e.templates[this.name + "_scaley_class"](this.y_unit[v].key, this.y_unit[v].label, this.y_unit[v]) ? " " + e.templates[this.name + "_scaley_class"](this.y_unit[v].key, this.y_unit[v].label, this.y_unit[v]) : "")), y.td_content || (y.td_content = e.templates[this.name + "_scale_label"](this.y_unit[v].key, this.y_unit[v].label, this.y_unit[v])), e._merge(y, {
                        tr_className: "",
                        style_height: "height:" + y.height + "px;",
                        style_width: "width:" + (this.dx - 1) + "px;",
                        summ_width: "width:" + p + "px;",
                        table_className: ""
                    });
                var x = r.call(this, i[v]);
                if (this.fit_events) {
                    var k = this._events_height[this.y_unit[v].key] || 0;
                    y.height = k > y.height ? k : y.height, y.style_height = "height:" + y.height + "px;", this._section_height[this.y_unit[v].key] = y.height
                }
                if (n += "<tr class='" + y.tr_className + "' style='" + y.style_height + "'><td class='" + y.td_className + "' style='" + y.style_width + " height:" + (y.height - 1) + "px;'>" + y.td_content + "</td>", "cell" == this.render)
                    for (var o = 0; o < e._cols.length; o++) n += e._ignores[o] ? "<td></td>" : "<td class='dhx_matrix_cell " + e.templates[this.name + "_cell_class"](i[v][o], this._trace_x[o], this.y_unit[v]) + "' style='width:" + (e._cols[o] - 1) + "px'><div style='width:" + (e._cols[o] - 1) + "px'>" + e.templates[this.name + "_cell_value"](i[v][o], this._trace_x[o], this.y_unit[v]) + "</div></td>";
                else {
                    n += "<td><div style='" + y.summ_width + " " + y.style_height + " position:relative;' class='dhx_matrix_line'>", n += x, n += "<table class='" + y.table_className + "' cellpadding='0' cellspacing='0' style='" + y.summ_width + " " + y.style_height + "' >";
                    for (var o = 0; o < e._cols.length; o++) n += e._ignores[o] ? "<td></td>" : "<td class='dhx_matrix_cell " + e.templates[this.name + "_cell_class"](i[v], this._trace_x[o], this.y_unit[v]) + "' style='width:" + (e._cols[o] - 1) + "px'><div style='width:" + (e._cols[o] - 1) + "px'></div></td>";
                    n += "</table>", n += "</div></td>";
                }
                n += "</tr>", b.push(y)
            }
            n += "</table>", this._matrix = i, a.innerHTML = n, e._rendered = [];
            for (var w = e._obj.getElementsByTagName("DIV"), v = 0; v < w.length; v++) w[v].getAttribute("event_id") && e._rendered.push(w[v]);
            this._scales = {};
            for (var D = a.firstChild.rows, E = null, v = 0, M = b.length; M > v; v++) {
                E = this.y_unit[v], f.push(b[v].height);
                var S = E.key,
                    N = this._scales[S] = e._isRender("cell") ? D[v] : D[v].childNodes[1].getElementsByTagName("div")[0];
                e.callEvent("onScaleAdd", [N, S])
            }
        }

        function l(t) {
            var a = e._min_date,
                n = e._max_date;
            e._process_ignores(a, this.x_size, this.x_unit, this.x_step, t);
            for (var i = (this.x_size + (t ? e._ignores_detected : 0), 0), r = 0; +n > +a;)
                if (this._trace_x[r] = new Date(a), a = e.date.add(a, this.x_step, this.x_unit), e.date[this.x_unit + "_start"] && (a = e.date[this.x_unit + "_start"](a)), e._ignores[r] || i++, r++, t)
                    if (i < this.x_size && !(+n > +a)) n = e.date["add_" + this.name + "_private"](n, (this.x_length || this.x_size) * this.x_step);
                    else if (i >= this.x_size) {
                        e._max_date = a;
                        break
                    }
            return {
                total: r,
                displayed: i
            }
        }

        function o(t) {
            var a = e.xy.scale_height,
                n = this._header_resized || e.xy.scale_height;
            e._cols = [], e._colsS = {
                height: 0
            }, this._trace_x = [];
            var i = e._x - this.dx - e.xy.scroll_width,
                r = [this.dx],
                d = e._els.dhx_cal_header[0];
            d.style.width = r[0] + i + "px";
            for (var o = e._min_date_timeline = e._min_date, _ = e.config.preserve_scale_length, c = l.call(this, _), u = c.displayed, h = c.total, p = 0; h > p; p++) e._ignores[p] ? (e._cols[p] = 0, u++) : e._cols[p] = Math.floor(i / (u - p)), i -= e._cols[p], r[p + 1] = r[p] + e._cols[p];
            if (t.innerHTML = "<div></div>", this.second_scale) {
                for (var m = this.second_scale.x_unit, g = [this._trace_x[0]], f = [], b = [this.dx, this.dx], y = 0, x = 0; x < this._trace_x.length; x++) {
                    var k = this._trace_x[x],
                        w = s(m, k, g[y]);
                    w && (++y, g[y] = k, b[y + 1] = b[y]);
                    var D = y + 1;
                    f[y] = e._cols[x] + (f[y] || 0), b[D] += e._cols[x]
                }
                t.innerHTML = "<div></div><div></div>";
                var E = t.firstChild;
                E.style.height = n + "px";
                var M = t.lastChild;
                M.style.position = "relative";
                for (var S = 0; S < g.length; S++) {
                    var N = g[S],
                        O = e.templates[this.name + "_second_scalex_class"](N),
                        T = document.createElement("DIV");
                    T.className = "dhx_scale_bar dhx_second_scale_bar" + (O ? " " + O : ""), e.set_xy(T, f[S] - 1, n - 3, b[S], 0), T.innerHTML = e.templates[this.name + "_second_scale_date"](N),
                        E.appendChild(T)
                }
            }
            e.xy.scale_height = n, t = t.lastChild;
            for (var C = 0; C < this._trace_x.length; C++)
                if (!e._ignores[C]) {
                    o = this._trace_x[C], e._render_x_header(C, r[C], o, t);
                    var L = e.templates[this.name + "_scalex_class"](o);
                    L && (t.lastChild.className += " " + L)
                }
            e.xy.scale_height = a;
            var A = this._trace_x;
            t.onclick = function (t) {
                var a = v(t);
                a && e.callEvent("onXScaleClick", [a.x, A[a.x], t || event])
            }, t.ondblclick = function (t) {
                var a = v(t);
                a && e.callEvent("onXScaleDblClick", [a.x, A[a.x], t || event])
            }
        }

        function s(t, a, n) {
            switch (t) {
                case "hour":
                    return a.getHours() != n.getHours() || s("day", a, n);
                case "day":
                    return !(a.getDate() == n.getDate() && a.getMonth() == n.getMonth() && a.getFullYear() == n.getFullYear());
                case "week":
                    return !(e.date.week_start(new Date(a)).valueOf() == e.date.week_start(new Date(n)).valueOf());
                case "month":
                    return !(a.getMonth() == n.getMonth() && a.getFullYear() == n.getFullYear());
                case "year":
                    return !(a.getFullYear() == n.getFullYear());
                default:
                    return !1
            }
        }

        function _(t) {
            if (this._header_resized && (!t || !this.second_scale)) {
                e.xy.scale_height /= 2, this._header_resized = !1;
                var a = e._els.dhx_cal_header[0];
                a.className = a.className.replace(/ dhx_second_cal_header/gi, "")
            }
        }

        function c(t) {
            if (_.call(this, t), t) {
                this.second_scale && !this._header_resized && (this._header_resized = e.xy.scale_height, e.xy.scale_height *= 2, e._els.dhx_cal_header[0].className += " dhx_second_cal_header"), e.set_sizes(), e._init_matrix_tooltip();
                var a = e._min_date;
                o.call(this, e._els.dhx_cal_header[0]), d.call(this, e._els.dhx_cal_data[0]), e._min_date = a, e._els.dhx_cal_date[0].innerHTML = e.templates[this.name + "_date"](e._min_date, e._max_date),
                    e._mark_now && e._mark_now(), _.call(this, t)
            }
            u()
        }

        function u() {
            e._tooltip && (e._tooltip.style.display = "none", e._tooltip.date = "")
        }

        function h(t, a, n) {
            if ("cell" == t.render) {
                var i = a.x + "_" + a.y,
                    r = t._matrix[a.y][a.x];
                if (!r) return u();
                if (r.sort(function (e, t) {
                        return e.start_date > t.start_date ? 1 : -1
                }), e._tooltip) {
                    if (e._tooltip.date == i) return;
                    e._tooltip.innerHTML = ""
                } else {
                    var d = e._tooltip = document.createElement("DIV");
                    d.className = "dhx_year_tooltip", document.body.appendChild(d), d.onclick = e._click.dhx_cal_data
                }
                for (var l = "", o = 0; o < r.length; o++) {
                    var s = r[o].color ? "background-color:" + r[o].color + ";" : "",
                        _ = r[o].textColor ? "color:" + r[o].textColor + ";" : "";
                    l += "<div class='dhx_tooltip_line' event_id='" + r[o].id + "' style='" + s + _ + "'>", l += "<div class='dhx_tooltip_date'>" + (r[o]._timed ? e.templates.event_date(r[o].start_date) : "") + "</div>", l += "<div class='dhx_event_icon icon_details'>&nbsp;</div>", l += e.templates[t.name + "_tooltip"](r[o].start_date, r[o].end_date, r[o]) + "</div>"
                }
                e._tooltip.style.display = "", e._tooltip.style.top = "0px", document.body.offsetWidth - n.left - e._tooltip.offsetWidth < 0 ? e._tooltip.style.left = n.left - e._tooltip.offsetWidth + "px" : e._tooltip.style.left = n.left + a.src.offsetWidth + "px",
                    e._tooltip.date = i, e._tooltip.innerHTML = l, document.body.offsetHeight - n.top - e._tooltip.offsetHeight < 0 ? e._tooltip.style.top = n.top - e._tooltip.offsetHeight + a.src.offsetHeight + "px" : e._tooltip.style.top = n.top + "px"
            }
        }

        function p(e) {
            for (var t = e.parentNode.childNodes, a = 0; a < t.length; a++)
                if (t[a] == e) return a;
            return -1
        }

        function v(e) {
            e = e || event;
            for (var t = e.target ? e.target : e.srcElement; t && "DIV" != t.tagName;) t = t.parentNode;
            if (t && "DIV" == t.tagName) {
                var a = t.className.split(" ")[0];
                if ("dhx_scale_bar" == a) return {
                    x: p(t),
                    y: -1,
                    src: t,
                    scale: !0
                }
            }
        }
        e.matrix = {}, e._merge = function (e, t) {
            for (var a in t) "undefined" == typeof e[a] && (e[a] = t[a])
        }, e.createTimelineView = function (t) {
            e._skin_init(), e._merge(t, {
                section_autoheight: !0,
                name: "matrix",
                x: "time",
                y: "time",
                x_step: 1,
                x_unit: "hour",
                y_unit: "day",
                y_step: 1,
                x_start: 0,
                x_size: 24,
                y_start: 0,
                y_size: 7,
                render: "cell",
                dx: 200,
                dy: 50,
                event_dy: e.xy.bar_height - 5,
                event_min_dy: e.xy.bar_height - 5,
                resize_events: !0,
                fit_events: !0,
                show_unassigned: !1,
                second_scale: !1,
                round_position: !1,
                _logic: function (t, a, n) {
                    var i = {};
                    return e.checkEvent("onBeforeSectionRender") && (i = e.callEvent("onBeforeSectionRender", [t, a, n])), i
                }
            }), t._original_x_start = t.x_start, "day" != t.x_unit && (t.first_hour = t.last_hour = 0), t._start_correction = t.first_hour ? 60 * t.first_hour * 60 * 1e3 : 0, t._end_correction = t.last_hour ? 60 * (24 - t.last_hour) * 60 * 1e3 : 0, e.checkEvent("onTimelineCreated") && e.callEvent("onTimelineCreated", [t]);
            var a = e.render_data;
            e.render_data = function (n, i) {
                if (this._mode != t.name) return a.apply(this, arguments);
                if (i && !t.show_unassigned && "cell" != t.render)
                    for (var r = 0; r < n.length; r++) this.clear_event(n[r]),
                        this.render_timeline_event.call(this.matrix[this._mode], n[r], !0);
                else e._renderMatrix.call(t, !0, !0)
            }, e.matrix[t.name] = t, e.templates[t.name + "_cell_value"] = function (e) {
                return e ? e.length : ""
            }, e.templates[t.name + "_cell_class"] = function (e) {
                return ""
            }, e.templates[t.name + "_scalex_class"] = function (e) {
                return ""
            }, e.templates[t.name + "_second_scalex_class"] = function (e) {
                return ""
            }, e.templates[t.name + "_scaley_class"] = function (e, t, a) {
                return ""
            }, e.templates[t.name + "_scale_label"] = function (e, t, a) {
                return t
            }, e.templates[t.name + "_tooltip"] = function (e, t, a) {
                return a.text
            }, e.templates[t.name + "_date"] = function (t, a) {
                return t.getDay() == a.getDay() && 864e5 > a - t || +t == +e.date.date_part(new Date(a)) || +e.date.add(t, 1, "day") == +a && 0 === a.getHours() && 0 === a.getMinutes() ? e.templates.day_date(t) : t.getDay() != a.getDay() && 864e5 > a - t ? e.templates.day_date(t) + " &ndash; " + e.templates.day_date(a) : e.templates.week_date(t, a)
            }, e.templates[t.name + "_scale_date"] = e.date.date_to_str(t.x_date || e.config.hour_date), e.templates[t.name + "_second_scale_date"] = e.date.date_to_str(t.second_scale && t.second_scale.x_date ? t.second_scale.x_date : e.config.hour_date),
                e.date["add_" + t.name + "_private"] = function (a, n) {
                    var i = n,
                        r = t.x_unit;
                    if ("minute" == t.x_unit || "hour" == t.x_unit) {
                        var d = i;
                        "hour" == t.x_unit && (d *= 60), d % 1440 || (i = d / 1440, r = "day")
                    }
                    return e.date.add(a, i, r)
                }, e.date["add_" + t.name] = function (a, n, i) {
                    var r = e.date["add_" + t.name + "_private"](a, (t.x_length || t.x_size) * t.x_step * n);
                    if ("minute" == t.x_unit || "hour" == t.x_unit) {
                        var d = t.x_length || t.x_size,
                            l = "hour" == t.x_unit ? 60 * t.x_step : t.x_step;
                        if (l * d % 1440)
                            if (+e.date.date_part(new Date(a)) == +e.date.date_part(new Date(r))) t.x_start += n * d;
                            else {
                                var o = 1440 / (d * l) - 1,
                                    s = Math.round(o * d);
                                n > 0 ? t.x_start = t.x_start - s : t.x_start = s + t.x_start
                            }
                    }
                    return r
                }, e.date[t.name + "_start"] = function (a) {
                    var n = e.date[t.x_unit + "_start"] || e.date.day_start,
                        i = n.call(e.date, a);
                    return i = e.date.add(i, t.x_step * t.x_start, t.x_unit)
                }, e.callEvent("onOptionsLoad", [t]), e[t.name + "_view"] = function (a) {
                    a ? e._set_timeline_dates(t) : e._renderMatrix.apply(t, arguments)
                };
            var i = new Date;
            e.date.add(i, t.x_step, t.x_unit).valueOf() - i.valueOf();
            e["mouse_" + t.name] = function (a) {
                var i = this._drag_event;
                this._drag_id && (i = this.getEvent(this._drag_id)), a.x -= t.dx;
                var r = e._timeline_drag_date(t, a.x);
                if (a.x = 0, a.force_redraw = !0, a.custom = !0, "move" == this._drag_mode && this._drag_id && this._drag_event) {
                    var i = this.getEvent(this._drag_id),
                        d = this._drag_event;
                    if (a._ignores = this._ignores_detected || t._start_correction || t._end_correction, void 0 === d._move_delta && (d._move_delta = (i.start_date - r) / 6e4, this.config.preserve_length && a._ignores && (d._move_delta = this._get_real_event_length(i.start_date, r, t), d._event_length = this._get_real_event_length(i.start_date, i.end_date, t))),
                        this.config.preserve_length && a._ignores) {
                        var l = (d._event_length, this._get_fictional_event_length(r, d._move_delta, t, !0));
                        r = new Date(r - l)
                    } else r = e.date.add(r, d._move_delta, "minute")
                }
                if ("resize" == this._drag_mode && i && (!this.config.timeline_swap_resize && this._drag_id && (this._drag_from_start && +r > +i.end_date ? this._drag_from_start = !1 : !this._drag_from_start && +r < +i.start_date && (this._drag_from_start = !0)), a.resize_from_start = this._drag_from_start, !this.config.timeline_swap_resize && this._drag_id && this._drag_from_start && +r >= +e.date.add(i.end_date, -e.config.time_step, "minute") && (r = e.date.add(i.end_date, -e.config.time_step, "minute"))),
                    t.round_position) switch (this._drag_mode) {
                        case "move":
                            this.config.preserve_length || (r = n.call(t, r, !1), "day" == t.x_unit && (a.custom = !1));
                            break;
                        case "resize":
                            this._drag_event && ((null === this._drag_event._resize_from_start || void 0 === this._drag_event._resize_from_start) && (this._drag_event._resize_from_start = a.resize_from_start), a.resize_from_start = this._drag_event._resize_from_start, r = n.call(t, r, !this._drag_event._resize_from_start))
                    }
                this._resolve_timeline_section(t, a), a.section && this._update_timeline_section({
                    pos: a,
                    event: this.getEvent(this._drag_id),
                    view: t
                }), a.y = Math.round((this._correct_shift(r, 1) - this._min_date) / (6e4 * this.config.time_step)), a.shift = this.config.time_step;
                var o = this._is_pos_changed(this._drag_pos, a);
                return this._drag_pos && o && (this._drag_event._dhx_changed = !0), o || this._drag_pos.has_moved || (a.force_redraw = !1), a
            }
        }, e._get_timeline_event_height = function (e, t) {
            var a = e[t.y_property],
                n = t.event_dy;
            return "full" == t.event_dy && (n = t.section_autoheight ? t._section_height[a] - 6 : t.dy - 3), t.resize_events && (n = Math.max(Math.floor(n / e._count), t.event_min_dy)),
                n
        }, e._get_timeline_event_y = function (t, a) {
            var n = t,
                i = 2 + n * a + (n ? 2 * n : 0);
            return e.config.cascade_event_display && (i = 2 + n * e.config.cascade_event_margin + (n ? 2 * n : 0)), i
        }, e.render_timeline_event = function (t, n) {
            var i = t[this.y_property];
            if (!i) return "";
            var r = t._sorder,
                d = a(t, !1, this),
                l = a(t, !0, this),
                o = e._get_timeline_event_height(t, this),
                s = o - 2;
            t._inner || "full" != this.event_dy || (s = (s + 2) * (t._count - r) - 2);
            var _ = e._get_timeline_event_y(t._sorder, o),
                c = o + _ + 2;
            (!this._events_height[i] || this._events_height[i] < c) && (this._events_height[i] = c);
            var u = e.templates.event_class(t.start_date, t.end_date, t);
            u = "dhx_cal_event_line " + (u || ""), t._no_drag_move && (u += " no_drag_move");
            var h = t.color ? "background:" + t.color + ";" : "",
                p = t.textColor ? "color:" + t.textColor + ";" : "",
                v = e.templates.event_bar_text(t.start_date, t.end_date, t),
                m = '<div event_id="' + t.id + '" class="' + u + '" style="' + h + p + "position:absolute; top:" + _ + "px; height: " + s + "px; left:" + d + "px; width:" + Math.max(0, l - d) + "px;" + (t._text_style || "") + '">';
            if (e.config.drag_resize && !e.config.readonly) {
                var g = "dhx_event_resize",
                    f = "<div class='" + g + " " + g + "_start' style='height: " + s + "px;'></div>",
                    b = "<div class='" + g + " " + g + "_end' style='height: " + s + "px;'></div>";
                m += (t._no_resize_start ? "" : f) + (t._no_resize_end ? "" : b)
            }
            if (m += v + "</div>", !n) return m;
            var y = document.createElement("DIV");
            y.innerHTML = m;
            var x = this.order[i],
                k = e._els.dhx_cal_data[0].firstChild.rows[x];
            if (k) {
                var w = k.cells[1].firstChild;
                e._rendered.push(y.firstChild), w.appendChild(y.firstChild)
            }
        }, e._matrix_tooltip_handler = function (t) {
            var a = e.matrix[e._mode];
            if (a && "cell" == a.render) {
                if (a) {
                    var n = e._locate_cell_timeline(t),
                        t = t || event;
                    t.target || t.srcElement;
                    if (n) return h(a, n, getOffset(n.src))
                }
                u()
            }
        }, e._init_matrix_tooltip = function () {
            e._detachDomEvent(e._els.dhx_cal_data[0], "mouseover", e._matrix_tooltip_handler), dhtmlxEvent(e._els.dhx_cal_data[0], "mouseover", e._matrix_tooltip_handler)
        }, e._set_timeline_dates = function (t) {
            e._min_date = e.date[t.name + "_start"](new Date(e._date)), e._max_date = e.date["add_" + t.name + "_private"](e._min_date, t.x_size * t.x_step), e.date[t.x_unit + "_start"] && (e._max_date = e.date[t.x_unit + "_start"](e._max_date)), e._table_view = !0
        }, e._renderMatrix = function (t, a) {
            a || (e._els.dhx_cal_data[0].scrollTop = 0), e._set_timeline_dates(this),
                c.call(this, t)
        }, e._locate_cell_timeline = function (t) {
            t = t || event;
            for (var a = t.target ? t.target : t.srcElement, n = {}, i = e.matrix[e._mode], r = e.getActionData(t), d = e._ignores, l = 0, o = 0; o < i._trace_x.length - 1 && !(+r.date < i._trace_x[o + 1]) ; o++) d[o] || l++;
            n.x = 0 === l ? 0 : o, n.y = i.order[r.section];
            var s = e._isRender("cell") ? 1 : 0;
            n.src = i._scales[r.section] ? i._scales[r.section].getElementsByTagName("td")[o + s] : null;
            for (var _ = !1; 0 === n.x && "dhx_cal_data" != a.className && a.parentNode;) {
                if ("dhx_matrix_scell" == a.className.split(" ")[0]) {
                    _ = !0;
                    break
                }
                a = a.parentNode
            }
            return _ ? (n.x = -1, n.src = a, n.scale = !0) : n.x = o, n
        };
        var m = e._click.dhx_cal_data;
        e._click.dhx_marked_timespan = e._click.dhx_cal_data = function (t) {
            var a = m.apply(this, arguments),
                n = e.matrix[e._mode];
            if (n) {
                var i = e._locate_cell_timeline(t);
                i && (i.scale ? e.callEvent("onYScaleClick", [i.y, n.y_unit[i.y], t || event]) : e.callEvent("onCellClick", [i.x, i.y, n._trace_x[i.x], (n._matrix[i.y] || {})[i.x] || [], t || event]))
            }
            return a
        }, e.dblclick_dhx_matrix_cell = function (t) {
            var a = e.matrix[e._mode];
            if (a) {
                var n = e._locate_cell_timeline(t);
                n && (n.scale ? e.callEvent("onYScaleDblClick", [n.y, a.y_unit[n.y], t || event]) : e.callEvent("onCellDblClick", [n.x, n.y, a._trace_x[n.x], (a._matrix[n.y] || {})[n.x] || [], t || event]))
            }
        };
        var g = e.dblclick_dhx_marked_timespan || function () { };
        e.dblclick_dhx_marked_timespan = function (t) {
            var a = e.matrix[e._mode];
            return a ? e.dblclick_dhx_matrix_cell(t) : g.apply(this, arguments)
        }, e.dblclick_dhx_matrix_scell = function (t) {
            return e.dblclick_dhx_matrix_cell(t)
        }, e._isRender = function (t) {
            return e.matrix[e._mode] && e.matrix[e._mode].render == t;
        }, e.attachEvent("onCellDblClick", function (t, a, n, i, r) {
            if (!this.config.readonly && ("dblclick" != r.type || this.config.dblclick_create)) {
                var d = e.matrix[e._mode],
                    l = {};
                l.start_date = d._trace_x[t], l.end_date = d._trace_x[t + 1] ? d._trace_x[t + 1] : e.date.add(d._trace_x[t], d.x_step, d.x_unit), d._start_correction && (l.start_date = new Date(1 * l.start_date + d._start_correction)), d._end_correction && (l.end_date = new Date(l.end_date - d._end_correction)), l[d.y_property] = d.y_unit[a].key, e.addEventNow(l, null, r)
            }
        }), e.attachEvent("onBeforeDrag", function (t, a, n) {
            return !e._isRender("cell")
        }), e.attachEvent("onEventChanged", function (e, t) {
            t._timed = this.isOneDayEvent(t)
        }), e.attachEvent("onBeforeEventChanged", function (e, t, a, n) {
            return e && (e._move_delta = void 0), n && (n._move_delta = void 0), !0
        }), e._is_column_visible = function (t) {
            var a = e.matrix[e._mode],
                n = e._get_date_index(a, t);
            return !e._ignores[n]
        };
        var f = e._render_marked_timespan;
        e._render_marked_timespan = function (t, n, i, r, d) {
            if (!e.config.display_marked_timespans) return [];
            if (e.matrix && e.matrix[e._mode]) {
                if (e._isRender("cell")) return;
                var l = e._lame_copy({}, e.matrix[e._mode]);
                l.round_position = !1;
                var o = [],
                    s = [],
                    _ = [],
                    c = t.sections ? t.sections.units || t.sections.timeline : null;
                if (i) _ = [n], s = [i];
                else {
                    var u = l.order;
                    if (c) u.hasOwnProperty(c) && (s.push(c), _.push(l._scales[c]));
                    else
                        for (var h in u) u.hasOwnProperty(h) && (s.push(h), _.push(l._scales[h]))
                }
                var r = r ? new Date(r) : e._min_date,
                    d = d ? new Date(d) : e._max_date;
                r.valueOf() < e._min_date.valueOf() && (r = new Date(e._min_date)), d.valueOf() > e._max_date.valueOf() && (d = new Date(e._max_date));
                for (var p = 0; p < l._trace_x.length && !e._is_column_visible(l._trace_x[p]) ; p++);
                if (p == l._trace_x.length) return;
                var v = [];
                if (t.days > 6) {
                    var m = new Date(t.days);
                    e.date.date_part(new Date(r)) <= +m && +d >= +m && v.push(m)
                } else v.push.apply(v, e._get_dates_by_index(t.days));
                for (var g = t.zones, b = e._get_css_classes_by_config(t), y = 0; y < s.length; y++) {
                    n = _[y], i = s[y];
                    for (var p = 0; p < v.length; p++)
                        for (var x = v[p], k = 0; k < g.length; k += 2) {
                            var w = g[k],
                                D = g[k + 1],
                                E = new Date(+x + 60 * w * 1e3),
                                M = new Date(+x + 60 * D * 1e3);
                            if (M > r && d > E) {
                                var S = e._get_block_by_config(t);
                                S.className = b;
                                var N = a({
                                    start_date: E
                                }, !1, l) - 1,
                                    O = a({
                                        start_date: M
                                    }, !1, l) - 1,
                                    T = Math.max(1, O - N - 1),
                                    C = l._section_height[i] - 1 || l.dy - 1;
                                S.style.cssText = "height: " + C + "px; left: " + N + "px; width: " + T + "px; top: 0;", n.insertBefore(S, n.firstChild), o.push(S)
                            }
                        }
                }
                return o
            }
            return f.apply(e, [t, n, i])
        };
        var b = e._append_mark_now;
        e._append_mark_now = function (t, a) {
            if (e.matrix && e.matrix[e._mode]) {
                var n = e._currentDate(),
                    i = e._get_zone_minutes(n),
                    r = {
                        days: +e.date.date_part(n),
                        zones: [i, i + 1],
                        css: "dhx_matrix_now_time",
                        type: "dhx_now_time"
                    };
                return e._render_marked_timespan(r)
            }
            return b.apply(e, [t, a]);
        };
        var y = e._mark_timespans;
        e._mark_timespans = function () {
            if (e.matrix && e.matrix[e.getState().mode]) {
                for (var t = [], a = e.matrix[e.getState().mode], n = a.y_unit, i = 0; i < n.length; i++) {
                    var r = n[i].key,
                        d = a._scales[r],
                        l = e._on_scale_add_marker(d, r);
                    t.push.apply(t, l)
                }
                return t
            }
            return y.apply(this, arguments)
        };
        var x = e._on_scale_add_marker;
        e._on_scale_add_marker = function (t, a) {
            if (e.matrix && e.matrix[e._mode]) {
                var n = [],
                    i = e._marked_timespans;
                if (i && e.matrix && e.matrix[e._mode])
                    for (var r = e._mode, d = e._min_date, l = e._max_date, o = i.global, s = e.date.date_part(new Date(d)) ; l > s; s = e.date.add(s, 1, "day")) {
                        var _ = +s,
                            c = s.getDay(),
                            u = [],
                            h = o[_] || o[c];
                        if (u.push.apply(u, e._get_configs_to_render(h)), i[r] && i[r][a]) {
                            var p = [],
                                v = e._get_types_to_render(i[r][a][c], i[r][a][_]);
                            p.push.apply(p, e._get_configs_to_render(v)), p.length && (u = p)
                        }
                        for (var m = 0; m < u.length; m++) {
                            var g = u[m],
                                f = g.days;
                            7 > f ? (f = _, n.push.apply(n, e._render_marked_timespan(g, t, a, s, e.date.add(s, 1, "day"))), f = c) : n.push.apply(n, e._render_marked_timespan(g, t, a, s, e.date.add(s, 1, "day")))
                        }
                    }
                return n
            }
            return x.apply(this, arguments)
        }, e._resolve_timeline_section = function (e, t) {
            var a = 0,
                n = 0;
            for (a; a < this._colsS.heights.length && (n += this._colsS.heights[a], !(n > t.y)) ; a++);
            e.y_unit[a] || (a = e.y_unit.length - 1), this._drag_event && !this._drag_event._orig_section && (this._drag_event._orig_section = e.y_unit[a].key), t.fields = {}, a >= 0 && e.y_unit[a] && (t.section = t.fields[e.y_property] = e.y_unit[a].key)
        }, e._update_timeline_section = function (e) {
            var t = e.view,
                a = e.event,
                n = e.pos;
            if (a) {
                if (a[t.y_property] != n.section) {
                    var i = this._get_timeline_event_height(a, t);
                    a._sorder = this._get_dnd_order(a._sorder, i, t._section_height[n.section]);
                }
                a[t.y_property] = n.section
            }
        }, e._get_date_index = function (e, t) {
            for (var a = 0, n = e._trace_x; a < n.length - 1 && +t >= +n[a + 1];) a++;
            return a
        }, e._timeline_drag_date = function (t, a) {
            var n, i, r = t,
                d = {
                    x: a
                },
                l = 0,
                o = 0;
            for (o; o <= this._cols.length - 1; o++)
                if (i = this._cols[o], l += i, l > d.x) {
                    n = (d.x - (l - i)) / i, n = 0 > n ? 0 : n;
                    break
                }
            if (r.round_position) {
                var s = 1;
                e.getState().drag_mode && "move" != e.getState().drag_mode && (s = .5), n >= s && o++, n = 0
            }
            if (0 === o && this._ignores[0])
                for (o = 1, n = 0; this._ignores[o];) o++;
            else if (o == this._cols.length && this._ignores[o - 1]) {
                for (o = this._cols.length - 1, n = 0; this._ignores[o];) o--;
                o++
            }
            var _;
            if (o >= r._trace_x.length) _ = e.date.add(r._trace_x[r._trace_x.length - 1], r.x_step, r.x_unit), r._end_correction && (_ = new Date(_ - r._end_correction));
            else {
                var c = n * i * r._step + r._start_correction;
                _ = new Date(+r._trace_x[o] + c)
            }
            return _
        }, e.attachEvent("onBeforeTodayDisplayed", function () {
            for (var t in e.matrix) {
                var a = e.matrix[t];
                a.x_start = a._original_x_start
            }
            return !0
        }), e.attachEvent("onOptionsLoad", function () {
            for (var t in e.matrix) {
                var a = e.matrix[t];
                a.order = {},
                    e.callEvent("onOptionsLoadStart", []);
                for (var t = 0; t < a.y_unit.length; t++) a.order[a.y_unit[t].key] = t;
                e.callEvent("onOptionsLoadFinal", []), e._date && a.name == e._mode && e.setCurrentView(e._date, e._mode)
            }
        }), e.attachEvent("onSchedulerResize", function () {
            if (e.matrix[this._mode]) {
                var t = e.matrix[this._mode];
                return e._renderMatrix.call(t, !0, !0), !1
            }
            return !0
        }), e.attachEvent("onBeforeDrag", function (t, a, n) {
            if ("resize" == a) {
                var i = n.target || n.srcElement;
                (i.className || "").indexOf("dhx_event_resize_end") < 0 ? e._drag_from_start = !0 : e._drag_from_start = !1;
            }
            return !0
        })
    }, e._temp_matrix_scope()
});
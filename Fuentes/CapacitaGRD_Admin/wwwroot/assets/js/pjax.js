// Navegación por AJAX: en vez de que cada clic recargue TODA la página (layout +
// todas las librerías + el loader rojo de pantalla completa), esto trae solo el
// contenido de la página destino y lo intercambia dentro de #pjax-content, dejando el
// layout (menú, header) siempre cargado.
//
// Ámbito: CUALQUIER link interno del sitio (no solo el menú principal), excepto los
// explícitamente excluidos abajo (login/logout, descargas/reportes, o cualquier link
// marcado con data-no-pjax). Si algo sale mal (respuesta que no es una página del
// sitio, red caída, etc.) cae a una navegación normal de respaldo — nunca deja al
// usuario a medias.
var Pjax = {
    contentSelector: "#pjax-content",
    progressSelector: "#pjax-progress",
    excludePatterns: [/\/login\b/i, /\/reporte\b/i, /\/exportar\b/i, /\/descargar\b/i],

    Init: function () {
        document.addEventListener("click", Pjax.OnClick);
        window.addEventListener("popstate", Pjax.OnPopState);
    },

    OnClick: function (e) {
        var link = e.target.closest("a[href]");
        if (!link) return;

        // Deja pasar: nueva pestaña, modificadores, links externos, anclas, togglers de
        // submenú, descargas, y cualquier cosa marcada explícitamente para no interceptar.
        if (e.defaultPrevented || e.button !== 0) return;
        if (e.ctrlKey || e.metaKey || e.shiftKey || e.altKey) return;
        if (link.target && link.target !== "" && link.target !== "_self") return;
        if (link.hasAttribute("download")) return;
        if (link.hasAttribute("data-no-pjax") || link.closest("[data-no-pjax]")) return;

        var href = link.getAttribute("href");
        if (!href || href.indexOf("javascript:") === 0 || href === "#") return;

        var url;
        try {
            url = new URL(href, window.location.href);
        } catch (err) {
            return;
        }
        if (url.origin !== window.location.origin) return;
        if (Pjax.excludePatterns.some(function (re) { return re.test(url.pathname); })) return;

        e.preventDefault();
        Pjax.Navigate(url.href, true);
    },

    OnPopState: function () {
        Pjax.Navigate(window.location.href, false);
    },

    Navigate: function (url, pushState) {
        Pjax.ShowProgress();

        fetch(url, { credentials: "same-origin" })
            .then(function (response) {
                if (!response.ok) throw new Error("HTTP " + response.status);
                return response.text();
            })
            .then(function (html) {
                var parsed = new DOMParser().parseFromString(html, "text/html");
                var newContent = parsed.querySelector(Pjax.contentSelector);
                var currentContent = document.querySelector(Pjax.contentSelector);

                if (!newContent || !currentContent) {
                    // La respuesta no tiene el layout esperado (p.ej. nos redirigió al
                    // login por sesión vencida) — navegar de verdad para no quedar a medias.
                    window.location.href = url;
                    return;
                }

                currentContent.innerHTML = newContent.innerHTML;
                if (parsed.title) document.title = parsed.title;

                Pjax.ExecuteScripts(currentContent);
                Pjax.UpdateActiveMenu(url);

                if (pushState) {
                    history.pushState({ pjax: true }, "", url);
                }

                window.scrollTo(0, 0);
                Pjax.HideProgress();
            })
            .catch(function () {
                // Cualquier falla (red, CORS, respuesta rara): navegación normal de respaldo,
                // nunca dejar al usuario con una pantalla a medio cargar.
                window.location.href = url;
            });
    },

    // Los <script> insertados vía innerHTML no se ejecutan solos; hay que recrearlos.
    // Se ejecutan en orden porque cada página espera que su script (persona.js, evento.js,
    // etc.) ya esté cargado antes del bloque inline que llama a Xxx.Init().
    ExecuteScripts: function (container) {
        var scripts = Array.prototype.slice.call(container.querySelectorAll("script"));

        function runNext(index) {
            if (index >= scripts.length) return;
            var old = scripts[index];
            var fresh = document.createElement("script");
            Array.prototype.forEach.call(old.attributes, function (attr) {
                fresh.setAttribute(attr.name, attr.value);
            });

            if (old.src) {
                fresh.onload = function () { runNext(index + 1); };
                fresh.onerror = function () { runNext(index + 1); };
                old.replaceWith(fresh);
            } else {
                fresh.textContent = old.textContent;
                old.replaceWith(fresh);
                runNext(index + 1);
            }
        }

        runNext(0);
    },

    UpdateActiveMenu: function (url) {
        var path = new URL(url).pathname.toLowerCase();
        document.querySelectorAll(".h-menu > li").forEach(function (li) {
            li.classList.remove("active", "open");
        });
        var link = Array.prototype.find.call(document.querySelectorAll(".h-menu a[href]"), function (a) {
            try {
                return new URL(a.href, window.location.href).pathname.toLowerCase() === path;
            } catch (e) {
                return false;
            }
        });
        if (link) {
            var li = link.closest(".h-menu > li") || link.closest("li");
            var topLi = link.closest(".h-menu > li");
            if (topLi) topLi.classList.add("active");
        }
    },

    ShowProgress: function () {
        var bar = document.querySelector(Pjax.progressSelector);
        if (!bar) return;
        bar.classList.add("pjax-loading");
        bar.style.width = "0%";
        // Progreso simulado: no sabemos el tamaño real de la respuesta, así que avanza
        // rápido al principio y se frena cerca del final hasta que la carga termine.
        requestAnimationFrame(function () { bar.style.width = "70%"; });
    },

    HideProgress: function () {
        var bar = document.querySelector(Pjax.progressSelector);
        if (!bar) return;
        bar.style.width = "100%";
        setTimeout(function () {
            bar.classList.remove("pjax-loading");
            bar.style.width = "0%";
        }, 200);
    },
};

Pjax.Init();

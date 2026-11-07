(() => {
    'use strict';
    const root = document.getElementById('analiticaVentas');
    if (!root) return;
    const $ = id => document.getElementById(id);
    const form = $('formFiltrosVenta');
    const inicio = $('fechaInicio'), fin = $('fechaFin');
    const cliente = form.querySelector('[name=clienteId]'), metodo = form.querySelector('[name=metodoPago]');
    const periodo = $('periodoAnalitica');
    let data, seleccionCategoria = null, seleccionProducto = null, charts = {}, requestId = 0;
    const paleta = ['#3B82F6','#EC4899','#10B981','#8B5CF6','#F59E0B','#06B6D4','#EF4444','#6366F1','#14B8A6','#F97316'];
    const colorCategoria = id => paleta[(Math.abs(Number(id)||1)-1)%paleta.length];
    const money = n => 'S/ ' + Number(n || 0).toLocaleString('es-PE', {minimumFractionDigits:2, maximumFractionDigits:2});
    const escape = s => String(s ?? '').replace(/[&<>"']/g, x => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[x]));
    const dateString = d => d.toISOString().slice(0,10);
    const fecha = s => new Date(s.slice(0,10) + 'T00:00:00Z');
    const sumarDias = (s,n) => { const d=fecha(s); d.setUTCDate(d.getUTCDate()+n); return dateString(d); };
    function rango() { return { start: inicio.value || null, end: fin.value || null }; }
    function establecerPeriodo() {
        const hoy=data.hoy, d=fecha(hoy), p=periodo.value;
        if(p==='custom') return;
        fin.value=p==='all'?'':hoy;
        inicio.value=p==='all'?'':p==='today'?hoy:['7','30','90'].includes(p)?sumarDias(hoy,1-Number(p)):p==='month'?hoy.slice(0,7)+'-01':d.getUTCFullYear()+'-01-01';
    }
    function detalles(v) {
        const suma=v.detalles.reduce((s,d)=>s+d.baseImporte,0);
        return v.detalles.map(d=>({...d, importe:suma ? v.total*d.baseImporte/suma : 0}));
    }
    function filtrar(start,end, aplicarSeleccion=true) {
        return data.ventas.filter(v => (!start || v.fecha.slice(0,10)>=start) && (!end || v.fecha.slice(0,10)<=end)).map(v=>{
            const todos=detalles(v), ds=todos.filter(d=>!aplicarSeleccion || ((!seleccionCategoria || d.categoriaId===seleccionCategoria) && (!seleccionProducto || d.productoId===seleccionProducto)));
            const seleccionado=aplicarSeleccion && (seleccionCategoria || seleccionProducto);
            return {...v, detalles:ds, importe:seleccionado ? ds.reduce((s,d)=>s+d.importe,0) : v.total};
        }).filter(v=>!(aplicarSeleccion && (seleccionCategoria || seleccionProducto)) || v.detalles.length);
    }
    const agrupar = (items,key) => { const m=new Map(); items.forEach(x=>{const k=key(x); if(!m.has(k))m.set(k,[]);m.get(k).push(x);}); return [...m.entries()]; };
    function resumen(ventas) {return { importe:ventas.reduce((s,v)=>s+v.importe,0), pedidos:ventas.length, clientes:new Set(ventas.map(v=>v.clienteId)).size, unidades:ventas.flatMap(v=>v.detalles).reduce((s,d)=>s+d.unidades,0) };}
    function anterior(r) { if(!r.start || !r.end) return null; const dias=Math.round((fecha(r.end)-fecha(r.start))/86400000)+1; return {start:sumarDias(r.start,-dias),end:sumarDias(r.start,-1)}; }
    function variacion(actual,previo, invertido=false) {if(previo==null || previo===0)return '<span class="kpi-change">Sin comparación disponible</span>'; const n=(actual-previo)/previo*100;return `<span class="kpi-change ${n===0?'':((n>0)!==invertido?'good':'bad')}">${n>0?'↑':n<0?'↓':'→'} ${Math.abs(n).toFixed(1)} % frente al periodo anterior</span>`;}
    function grafico(id, config, vacio, mensaje='No hay datos para mostrar en el periodo seleccionado.') {
        const canvas=$(id), aviso=canvas.parentElement.querySelector('.empty-chart');
        if(charts[id]) charts[id].destroy();
        aviso.hidden=!vacio; aviso.textContent=mensaje; canvas.style.visibility=vacio?'hidden':'visible';
        if(!vacio) charts[id]=new Chart(canvas,config);
    }
    const opcionesBase = () => ({responsive:true,maintainAspectRatio:false,plugins:{legend:{display:false}},scales:{x:{grid:{display:false},ticks:{color:'#8a95a5'}},y:{beginAtZero:true,grid:{color:'#e9edf5'},ticks:{color:'#8a95a5'}}}});
    function barras(id, rows, value, {horizontal=false,entero=false,click=null,tooltip=null}={}) {
        const opts=opcionesBase(); opts.indexAxis=horizontal?'y':'x'; const axis=horizontal?'x':'y'; opts.scales[axis].ticks.precision=entero?0:undefined; opts.scales[axis].beginAtZero=true;
        opts.onClick=(_e,els)=>{if(click&&els.length)click(rows[els[0].index]);};
        if(tooltip) opts.plugins.tooltip={callbacks:{label:ctx=>tooltip(rows[ctx.dataIndex])}};
        if(horizontal) opts.layout={padding:{right:45}};
        const etiquetas={id:'valoresYunan',afterDatasetsDraw(chart){if(!horizontal)return;const ctx=chart.ctx;ctx.save();ctx.font='11px Montserrat, sans-serif';ctx.fillStyle='#697588';chart.getDatasetMeta(0).data.forEach((bar,i)=>{ctx.fillText(String(value(rows[i])),bar.x+5,bar.y+4);});ctx.restore();}};
        grafico(id,{type:'bar',plugins:[etiquetas],data:{labels:rows.map(x=>x.nombre),datasets:[{label:entero?'Cantidad':'Importe',data:rows.map(value),backgroundColor:rows.map((x,i)=>x.unidades===0?'#eddde6':paleta[i%paleta.length]),borderRadius:5,barThickness:rows.length===1?24:undefined,maxBarThickness:30}]},options:opts},!rows.length);
    }
    function render() {
        if(!data || typeof Chart==='undefined')return;
        const r=rango(), ventas=filtrar(r.start,r.end), stats=resumen(ventas), prev=anterior(r), prevStats=prev?resumen(filtrar(prev.start,prev.end)):null;
        const detallesTodos=ventas.flatMap(v=>v.detalles);
        const catalogo=data.productos.filter(p=>(!seleccionCategoria||p.categoriaId===seleccionCategoria)&&(!seleccionProducto||p.id===seleccionProducto));
        const productos=catalogo.map(p=>{const ds=detallesTodos.filter(d=>d.productoId===p.id); return {...p,unidades:ds.reduce((s,d)=>s+d.unidades,0),importe:ds.reduce((s,d)=>s+d.importe,0)};});
        // Se incluyen productos vendidos que ya no estén activos en el ranking histórico.
        for(const [id,ds] of agrupar(detallesTodos,d=>d.productoId)) if(!productos.some(p=>p.id===id)) productos.push({id,nombre:ds[0].nombre,categoriaId:ds[0].categoriaId,categoria:ds[0].categoria,unidades:ds.reduce((s,d)=>s+d.unidades,0),importe:ds.reduce((s,d)=>s+d.importe,0),stock:null,critico:false});
        const sinVentas=productos.filter(p=>p.stock!==null && !p.unidades).length, criticos=productos.filter(p=>p.stock!==null&&p.critico).length;
        const ticket=stats.pedidos?stats.importe/stats.pedidos:0, prevTicket=prevStats?.pedidos?prevStats.importe/prevStats.pedidos:null;
        const cards=[['bi-currency-dollar','Ventas totales',money(stats.importe),'Importe final registrado',stats.importe,prevStats?.importe],['bi-bag-heart','Pedidos',stats.pedidos,'Ventas registradas',stats.pedidos,prevStats?.pedidos],['bi-people','Clientes',stats.clientes,'Clientes con compras',stats.clientes,prevStats?.clientes],['bi-tags','Productos vendidos',stats.unidades,'Unidades vendidas',stats.unidades,prevStats?.unidades],['bi-receipt','Ticket promedio',money(ticket),'Ingresos / pedidos',ticket,prevTicket],['bi-graph-up-arrow','Variación de ventas',prevStats?.importe?((stats.importe-prevStats.importe)/prevStats.importe*100).toFixed(1)+' %':'—','Respecto al periodo anterior',null,null],['bi-box-seam','Productos sin ventas',sinVentas,'Productos activos en el periodo',null,null],['bi-exclamation-triangle','Stock crítico',criticos,'Al menos una variante en el mínimo',null,null]];
        $('kpisAnalitica').innerHTML=[cards[0],cards[1],cards[3],cards[2],cards[4]].map(c=>`<div class="kpi-card"><span class="kpi-icon"><i class="bi ${c[0]}"></i></span><div class="kpi-label">${c[1]}</div><div class="kpi-value">${c[2]}</div><div class="kpi-description">${c[3]}</div>${variacion(c[4],c[5])}</div>`).join('');
        const selection=$('dashboardSelection'); selection.hidden=!seleccionCategoria&&!seleccionProducto; const elegido=data.productos.find(p=>seleccionProducto ? p.id===seleccionProducto : p.categoriaId===seleccionCategoria); selection.innerHTML=`Análisis: <strong>${escape(seleccionProducto?elegido?.nombre:elegido?.categoria)}</strong> · ${money(stats.importe)} · ${stats.unidades} unidades. <button type="button" id="clearSelection">Quitar selección</button> La tabla y el Excel conservan los filtros de fecha, cliente y pago.`; if(!selection.hidden)$('clearSelection').onclick=()=>{seleccionCategoria=null;seleccionProducto=null;render();};
        const agrupacion=$('agrupacion').value;
        const serie=agrupar(ventas,v=>{const f=v.fecha.slice(0,10);if(agrupacion==='month')return f.slice(0,7);if(agrupacion==='week'){const d=fecha(f), day=(d.getUTCDay()+6)%7;return sumarDias(f,-day);}return f;}).map(([nombre,vs])=>({nombre,importe:vs.reduce((s,v)=>s+v.importe,0),cantidad:vs.length})).sort((a,b)=>a.nombre.localeCompare(b.nombre));
        grafico('chartEvolucion',{type:'line',data:{labels:serie.map(x=>x.nombre),datasets:[{label:'Ingresos',data:serie.map(x=>x.importe),borderColor:'#3B82F6',backgroundColor:'rgba(59,130,246,.10)',fill:true,tension:.25,pointRadius:4}]},options:opcionesBase()},!serie.length,'No se registraron ventas durante el periodo seleccionado.');
        const categorias=agrupar(detallesTodos,d=>d.categoriaId).map(([id,ds])=>({id,nombre:ds[0].categoria,unidades:ds.reduce((s,d)=>s+d.unidades,0),importe:ds.reduce((s,d)=>s+d.importe,0)})).sort((a,b)=>b.importe-a.importe);
        const modoCat=$('modoCategoria').value, sumaCat=categorias.reduce((s,x)=>s+x[modoCat],0);
        const centro={id:'centroYunan',afterDraw(chart){const {ctx,chartArea}=chart;if(!chartArea)return;ctx.save();ctx.textAlign='center';ctx.fillStyle='#172B4D';ctx.font='bold 14px Montserrat, sans-serif';ctx.fillText(money(categorias.reduce((s,x)=>s+x.importe,0)),(chartArea.left+chartArea.right)/2,(chartArea.top+chartArea.bottom)/2);ctx.restore();}};
        grafico('chartCategorias',{type:'doughnut',plugins:[centro],data:{labels:categorias.map(x=>x.nombre),datasets:[{data:categorias.map(x=>x[modoCat]),backgroundColor:categorias.map(x=>colorCategoria(x.id)),borderWidth:2}]},options:{responsive:true,maintainAspectRatio:false,cutout:'65%',plugins:{legend:{position:'bottom',labels:{boxWidth:9,font:{size:10}}},tooltip:{callbacks:{label:c=>{const x=categorias[c.dataIndex];return `${x.nombre}: ${sumaCat?(x[modoCat]/sumaCat*100).toFixed(1):0}% · ${money(x.importe)} · ${x.unidades} unidades`;}}}},onClick:(_e,els)=>{if(els.length){seleccionCategoria=categorias[els[0].index].id;seleccionProducto=null;render();}}}},!categorias.length);
        const top=productos.filter(p=>p.unidades>0).sort((a,b)=>b.unidades-a.unidades||a.nombre.localeCompare(b.nombre)).slice(0,10), bajos=productos.filter(p=>p.stock!==null).sort((a,b)=>a.unidades-b.unidades||a.nombre.localeCompare(b.nombre)).slice(0,10);
        const alElegir=p=>{seleccionProducto=p.id;seleccionCategoria=p.categoriaId;render();}; const detalleTip=p=>`${p.unidades} unidades · ${money(p.importe)}${p.unidades===0?' · Sin ventas':''}`;
        barras('chartTop',top,p=>p.unidades,{horizontal:true,entero:true,click:alElegir,tooltip:detalleTip}); barras('chartBajos',bajos,p=>p.unidades,{horizontal:true,entero:true,click:alElegir,tooltip:detalleTip});
        const pagos=agrupar(ventas,v=>v.metodo).map(([nombre,vs])=>({nombre,transacciones:vs.length,importe:vs.reduce((s,v)=>s+v.importe,0)})); const modoPago=$('modoPago').value;
        barras('chartPagosNuevo',pagos,p=>modoPago==='porcentaje'?p.transacciones/stats.pedidos*100:p[modoPago],{horizontal:pagos.length===1,entero:modoPago==='transacciones',tooltip:p=>`${p.transacciones} ventas · ${money(p.importe)} · ${(p.transacciones/stats.pedidos*100).toFixed(1)} %`});
        const dias=['Lun','Mar','Mié','Jue','Vie','Sáb','Dom'].map((nombre,i)=>{const vs=ventas.filter(v=>(fecha(v.fecha).getUTCDay()+6)%7===i);return {nombre,transacciones:vs.length,importe:vs.reduce((s,v)=>s+v.importe,0)};}); barras('chartDias',ventas.length?dias:[],d=>d[$('modoDia').value],{entero:$('modoDia').value==='transacciones',tooltip:d=>`${d.transacciones} ventas · ${money(d.importe)}`});
        const meses=agrupar(ventas,v=>v.fecha.slice(0,7)).map(([nombre,vs])=>({nombre,importe:vs.reduce((s,v)=>s+v.importe,0)})).sort((a,b)=>a.nombre.localeCompare(b.nombre)); barras('chartMeses',meses,x=>x.importe,{tooltip:x=>{const d=fecha(x.nombre+'-01');d.setUTCMonth(d.getUTCMonth()-1);const prev=meses.find(m=>m.nombre===dateString(d).slice(0,7));return `${money(x.importe)} · ${prev?.importe?((x.importe-prev.importe)/prev.importe*100).toFixed(1)+' % vs. mes anterior':'Sin comparación disponible'}`;}});
        const reposicion=productos.filter(p=>p.critico&&p.unidades>0).sort((a,b)=>b.unidades-a.unidades); $('inventarioAnalitica').innerHTML=[['Productos con stock crítico',criticos],['Productos sin ventas en el periodo',sinVentas],['Producto con más unidades',top[0]?`${escape(top[0].nombre)} · ${top[0].unidades}`:'Sin datos'],['Posible reposición: vendidos y con stock crítico',reposicion.length],['Prioridad de reposición',reposicion[0]?`${escape(reposicion[0].nombre)} · stock ${reposicion[0].stock}`:'Sin candidatos']].map(x=>`<div class="inventory-line"><span>${x[0]}</span><strong>${x[1]}</strong></div>`).join('');
        const recurrentes=agrupar(ventas,v=>v.clienteId).filter(([,vs])=>vs.length>1).reduce((s,[,vs])=>s+vs.length,0);
        const insights=[['Producto destacado',top[0]?`${escape(top[0].nombre)} lidera con ${top[0].unidades} unidades y ${money(top[0].importe)}.`:'Sin ventas suficientes para identificar un producto destacado.','Informativo','Revisa su disponibilidad.'],['Categoría destacada',categorias[0]&&stats.importe?`${escape(categorias[0].nombre)} aporta ${(categorias[0].importe/stats.importe*100).toFixed(1)} % de los ingresos.`:'Sin datos suficientes para comparar categorías.','Informativo','Analiza su participación en el catálogo.'],['Alerta de inventario',`${criticos} productos tienen al menos una variante en stock mínimo o inferior.`,criticos?'Alta':'Informativo',criticos?'Revisa las variantes antes de reponer.':'Mantén el seguimiento del inventario.'],['Oportunidad comercial',`${sinVentas} productos activos no registraron ventas en el periodo.`,sinVentas?'Media':'Informativo','Revisa visibilidad, precio y disponibilidad.'],['Tendencia de ventas',prevStats?.importe?`Los ingresos variaron ${((stats.importe-prevStats.importe)/prevStats.importe*100).toFixed(1)} % respecto al periodo anterior equivalente.`:'Sin comparación disponible para este periodo.','Informativo','Compara periodos completos antes de tomar decisiones.'],['Clientes recurrentes',ventas.length?`${(recurrentes/ventas.length*100).toFixed(1)} % de las ventas pertenece a clientes con más de una compra en el periodo.`:'Sin ventas para analizar recurrencia.','Informativo','Considera acciones de fidelización.']];
        $('insightsAnalitica').innerHTML=insights.sort((a,b)=>({'Alta':0,'Media':1,'Informativo':2}[a[2]]-{'Alta':0,'Media':1,'Informativo':2}[b[2]])).slice(0,3).map(x=>`<article class="insight"><strong>${x[0]} · ${x[2]}</strong>${x[1]}<em>${x[3]}</em></article>`).join('');
    }
    async function cargar() {
        const id=++requestId; $('dashboardLoading').hidden=false; $('dashboardError').hidden=true;
        try {const query=new URLSearchParams();if(cliente.value)query.set('clienteId',cliente.value);if(metodo.value)query.set('metodoPago',metodo.value); const response=await fetch(root.dataset.url+'?'+query);if(!response.ok)throw new Error('No se pudo consultar la información.');const result=await response.json();if(id!==requestId)return;data=result;if(typeof Chart==='undefined')throw new Error('No se pudieron cargar los gráficos. Comprueba tu conexión.');$('dashboardContent').hidden=false;render();}
        catch(e){if(id===requestId){$('dashboardError').textContent=e.message;$('dashboardError').hidden=false;}}finally{if(id===requestId)$('dashboardLoading').hidden=true;}
    }
    periodo.value=inicio.value||fin.value?'custom':'all';
    let historialId=0;
    async function actualizarHistorial() {
        if(inicio.value&&fin.value&&inicio.value>fin.value)return;
        const id=++historialId, q=new URLSearchParams(new FormData(form));
        try {const response=await fetch(form.action+'?'+q);if(!response.ok)throw new Error();const html=await response.text();if(id!==historialId)return;const doc=new DOMParser().parseFromString(html,'text/html');const historial=doc.getElementById('historialVentas'), modales=doc.getElementById('modalesVentas');if(!historial||!modales)throw new Error();$('historialVentas').innerHTML=historial.innerHTML;$('modalesVentas').innerHTML=modales.innerHTML;}
        catch {if(id===historialId){$('dashboardError').textContent='No se pudo actualizar el historial. Usa Buscar para intentarlo de nuevo.';$('dashboardError').hidden=false;}}
    }
    periodo.addEventListener('change',()=>{if(!data)return;establecerPeriodo();render();actualizarHistorial();});
    [inicio,fin].forEach(x=>x.addEventListener('change',()=>{periodo.value='custom';if(inicio.value&&fin.value&&inicio.value>fin.value)return;render();actualizarHistorial();}));
    ['agrupacion','modoCategoria','modoPago','modoDia'].forEach(id=>$(id).addEventListener('change',render));
    $('resetAnalitica').onclick=()=>{form.reset();inicio.value='';fin.value='';cliente.value='';metodo.value='';periodo.value='all';seleccionCategoria=null;seleccionProducto=null;cargar();actualizarHistorial();};
    $('actualizarAnalitica').onclick=()=>{cargar();actualizarHistorial();};
    $('excelAnalitica').onclick=()=>{if(!validarFechasVenta())return;const q=new URLSearchParams();if(inicio.value)q.set('fechaInicio',inicio.value);if(fin.value)q.set('fechaFin',fin.value);if(cliente.value)q.set('clienteId',cliente.value);if(metodo.value)q.set('metodoPago',metodo.value);window.location.href=root.dataset.excel+'?'+q;};
    [cliente,metodo].forEach(x=>x.addEventListener('change',()=>{cargar();actualizarHistorial();}));
    form.addEventListener('submit',e=>{e.preventDefault();if(validarFechasVenta()){cargar();actualizarHistorial();}});
    cargar();
})();

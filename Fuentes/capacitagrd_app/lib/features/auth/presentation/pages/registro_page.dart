import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import '../../../../core/di/injection.dart';
import '../../../../shared/data/maestros_remote_datasource.dart';
import '../../../profile/data/models/persona_model.dart';
import '../../../profile/data/models/persona_data_model.dart';
import '../../../profile/presentation/bloc/persona_cubit.dart';

class RegistroPage extends StatefulWidget {
  const RegistroPage({super.key});

  @override
  State<RegistroPage> createState() => _RegistroPageState();
}

class _RegistroPageState extends State<RegistroPage> {
  final _formKeyDatos = GlobalKey<FormState>();
  final _formKeyLabor = GlobalKey<FormState>();

  final _dniCtrl = TextEditingController();
  final _nombresCtrl = TextEditingController();
  final _apPaternoCtrl = TextEditingController();
  final _apMaternoCtrl = TextEditingController();
  final _emailCtrl = TextEditingController();
  final _celularCtrl = TextEditingController();
  final _entidadOtraCtrl = TextEditingController();
  final _areaLaboraCtrl = TextEditingController();

  String _sexo = 'M';
  int _step = 0;
  DateTime? _fechaNacimiento;

  bool _cargandoMaestros = true;
  String? _errorMaestros;
  List<PaisItem> _paises = [];
  List<MaestroItem> _profesiones = [];
  List<MaestroItem> _entidades = [];
  List<MaestroItem> _cargos = [];
  List<DistritoItem> _distritos = [];
  List<String> _areasLaborales = [];

  int? _idPaisNacimiento;
  int? _idProfesion;
  int? _idPaisLabora;
  String? _departamentoLabora;
  String? _provinciaLabora;
  int? _idDistritoLabora;
  int? _idEntidad;
  int? _idCargo;

  @override
  void initState() {
    super.initState();
    _cargarMaestros();
  }

  Future<void> _cargarMaestros() async {
    final ds = getIt<MaestrosRemoteDataSource>();
    try {
      final results = await Future.wait([
        ds.getPaises(),
        ds.getProfesiones(),
        ds.getEntidades(),
        ds.getCargos(),
        ds.getDistritos(),
        ds.getAreasLaborales(),
      ]);
      if (!mounted) return;
      setState(() {
        _paises = results[0] as List<PaisItem>;
        _profesiones = (results[1] as List<MaestroItem>)..sort((a, b) => a.nombre.compareTo(b.nombre));
        _entidades = results[2] as List<MaestroItem>;
        _cargos = (results[3] as List<MaestroItem>)..sort((a, b) => a.nombre.compareTo(b.nombre));
        _distritos = results[4] as List<DistritoItem>;
        _areasLaborales = (results[5] as List<String>)..sort();
        final local = _paises.where((p) => p.esLocal).toList();
        final defaultPais = local.isNotEmpty ? local.first : (_paises.isNotEmpty ? _paises.first : null);
        _idPaisNacimiento = defaultPais?.id;
        _idPaisLabora = _idPaisNacimiento;
        _cargandoMaestros = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _errorMaestros = 'No se pudo cargar la información necesaria para el registro.';
        _cargandoMaestros = false;
      });
    }
  }

  List<String> get _departamentos =>
      _distritos.map((d) => d.departamento).toSet().toList()..sort();

  List<String> get _provincias => _distritos
      .where((d) => d.departamento == _departamentoLabora)
      .map((d) => d.provincia)
      .toSet()
      .toList()
    ..sort();

  List<DistritoItem> get _distritosFiltrados => _distritos
      .where((d) => d.departamento == _departamentoLabora && d.provincia == _provinciaLabora)
      .toList()
    ..sort((a, b) => a.distrito.compareTo(b.distrito));

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => getIt<PersonaCubit>(),
      child: Scaffold(
        appBar: AppBar(title: const Text('Registro de Participante')),
        body: _cargandoMaestros
            ? const Center(child: CircularProgressIndicator())
            : _errorMaestros != null
                ? Center(
                    child: Padding(
                      padding: const EdgeInsets.all(24),
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text(_errorMaestros!, textAlign: TextAlign.center),
                          const SizedBox(height: 16),
                          FilledButton(
                            onPressed: () => setState(() {
                              _cargandoMaestros = true;
                              _errorMaestros = null;
                              _cargarMaestros();
                            }),
                            child: const Text('Reintentar'),
                          ),
                        ],
                      ),
                    ),
                  )
                : BlocConsumer<PersonaCubit, PersonaState>(
                    listener: (context, state) {
                      if (state is PersonaCreadaExito) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(content: Text('¡Registro exitoso!'), backgroundColor: Colors.green),
                        );
                        context.go('/login');
                      }
                      if (state is PersonaError) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(content: Text(state.message), backgroundColor: Colors.red),
                        );
                      }
                    },
                    builder: (context, state) {
                      return Stepper(
                        currentStep: _step,
                        onStepContinue: () {
                          if (_step == 0 && _formKeyDatos.currentState!.validate()) {
                            if (_fechaNacimiento == null) {
                              ScaffoldMessenger.of(context).showSnackBar(
                                const SnackBar(content: Text('Selecciona tu fecha de nacimiento')),
                              );
                              return;
                            }
                            setState(() => _step = 1);
                          } else if (_step == 1 && _formKeyLabor.currentState!.validate()) {
                            if (_idDistritoLabora == null) {
                              ScaffoldMessenger.of(context).showSnackBar(
                                const SnackBar(content: Text('Selecciona el distrito donde laboras')),
                              );
                              return;
                            }
                            setState(() => _step = 2);
                          } else if (_step == 2) {
                            _registrar(context);
                          }
                        },
                        onStepCancel: () {
                          if (_step > 0) {
                            setState(() => _step--);
                          } else {
                            context.pop();
                          }
                        },
                        steps: [
                          Step(
                            title: const Text('Datos Personales'),
                            isActive: _step >= 0,
                            content: Form(key: _formKeyDatos, child: _buildDatosPersonales()),
                          ),
                          Step(
                            title: const Text('Datos Laborales'),
                            isActive: _step >= 1,
                            content: Form(key: _formKeyLabor, child: _buildDatosLaborales()),
                          ),
                          Step(
                            title: const Text('Confirmar'),
                            isActive: _step >= 2,
                            content: _buildConfirmar(state),
                          ),
                        ],
                      );
                    },
                  ),
      ),
    );
  }

  Widget _buildDatosPersonales() {
    return Column(
      children: [
        TextFormField(
          controller: _dniCtrl,
          decoration: const InputDecoration(labelText: 'DNI *'),
          keyboardType: TextInputType.number,
          maxLength: 8,
          validator: (v) => v == null || v.length != 8 ? 'DNI debe tener 8 dígitos' : null,
        ),
        TextFormField(
          controller: _nombresCtrl,
          decoration: const InputDecoration(labelText: 'Nombres *'),
          textCapitalization: TextCapitalization.words,
          validator: (v) => v == null || v.isEmpty ? 'Campo requerido' : null,
        ),
        TextFormField(
          controller: _apPaternoCtrl,
          decoration: const InputDecoration(labelText: 'Apellido Paterno *'),
          textCapitalization: TextCapitalization.words,
          validator: (v) => v == null || v.isEmpty ? 'Campo requerido' : null,
        ),
        TextFormField(
          controller: _apMaternoCtrl,
          decoration: const InputDecoration(labelText: 'Apellido Materno'),
          textCapitalization: TextCapitalization.words,
        ),
        ListTile(
          contentPadding: EdgeInsets.zero,
          title: Text(_fechaNacimiento == null
              ? 'Fecha de Nacimiento *'
              : 'Fecha de Nacimiento: ${DateFormat('dd/MM/yyyy').format(_fechaNacimiento!)}'),
          trailing: const Icon(Icons.calendar_today),
          onTap: () async {
            final now = DateTime.now();
            final picked = await showDatePicker(
              context: context,
              initialDate: _fechaNacimiento ?? DateTime(now.year - 18, now.month, now.day),
              firstDate: DateTime(now.year - 100),
              lastDate: now,
              // Arranca en modo "año" para no tener que ir mes por mes hasta llegar
              // al año de nacimiento (rango ahora de 100 años).
              initialDatePickerMode: DatePickerMode.year,
            );
            if (picked != null) setState(() => _fechaNacimiento = picked);
          },
        ),
        DropdownButtonFormField<int>(
          value: _idPaisNacimiento,
          decoration: const InputDecoration(labelText: 'Nacionalidad *'),
          isExpanded: true,
          items: _paises.map((p) => DropdownMenuItem(value: p.id, child: Text(p.nombre))).toList(),
          onChanged: (v) => setState(() => _idPaisNacimiento = v),
          validator: (v) => v == null ? 'Campo requerido' : null,
        ),
        DropdownButtonFormField<String>(
          value: _sexo,
          decoration: const InputDecoration(labelText: 'Sexo *'),
          items: const [
            DropdownMenuItem(value: 'M', child: Text('Masculino')),
            DropdownMenuItem(value: 'F', child: Text('Femenino')),
          ],
          onChanged: (v) => setState(() => _sexo = v ?? 'M'),
        ),
        _BuscadorMaestro(
          label: 'Profesión u ocupación *',
          opciones: _profesiones,
          seleccionado: _idProfesion,
          onSelected: (id) => setState(() => _idProfesion = id),
          validator: () => _idProfesion == null ? 'Campo requerido' : null,
          onAgregarNuevo: (nombre) async {
            final creado = await getIt<MaestrosRemoteDataSource>().crearProfesion(nombre);
            setState(() {
              _profesiones = [..._profesiones, creado]..sort((a, b) => a.nombre.compareTo(b.nombre));
            });
            return creado;
          },
        ),
        TextFormField(
          controller: _celularCtrl,
          decoration: const InputDecoration(labelText: 'N° Celular *'),
          keyboardType: TextInputType.phone,
          validator: (v) => v == null || v.isEmpty ? 'Campo requerido' : null,
        ),
        TextFormField(
          controller: _emailCtrl,
          decoration: const InputDecoration(labelText: 'Correo electrónico *'),
          keyboardType: TextInputType.emailAddress,
          validator: (v) => v == null || !v.contains('@') ? 'Correo inválido' : null,
        ),
      ],
    );
  }

  Widget _buildDatosLaborales() {
    return Column(
      children: [
        DropdownButtonFormField<int>(
          value: _idPaisLabora,
          decoration: const InputDecoration(labelText: 'País (donde labora) *'),
          isExpanded: true,
          items: _paises.map((p) => DropdownMenuItem(value: p.id, child: Text(p.nombre))).toList(),
          onChanged: (v) => setState(() => _idPaisLabora = v),
          validator: (v) => v == null ? 'Campo requerido' : null,
        ),
        DropdownButtonFormField<String>(
          value: _departamentoLabora,
          decoration: const InputDecoration(labelText: 'Departamento (donde labora) *'),
          isExpanded: true,
          items: _departamentos.map((d) => DropdownMenuItem(value: d, child: Text(d))).toList(),
          onChanged: (v) => setState(() {
            _departamentoLabora = v;
            _provinciaLabora = null;
            _idDistritoLabora = null;
          }),
          validator: (v) => v == null ? 'Campo requerido' : null,
        ),
        DropdownButtonFormField<String>(
          value: _provinciaLabora,
          decoration: const InputDecoration(labelText: 'Provincia (donde labora) *'),
          isExpanded: true,
          items: _provincias.map((p) => DropdownMenuItem(value: p, child: Text(p))).toList(),
          onChanged: _departamentoLabora == null
              ? null
              : (v) => setState(() {
                    _provinciaLabora = v;
                    _idDistritoLabora = null;
                  }),
          validator: (v) => v == null ? 'Campo requerido' : null,
        ),
        DropdownButtonFormField<int>(
          value: _idDistritoLabora,
          decoration: const InputDecoration(labelText: 'Distrito (donde labora) *'),
          isExpanded: true,
          items: _distritosFiltrados
              .map((d) => DropdownMenuItem(value: d.idDistrito, child: Text(d.distrito)))
              .toList(),
          onChanged: _provinciaLabora == null ? null : (v) => setState(() => _idDistritoLabora = v),
          validator: (v) => v == null ? 'Campo requerido' : null,
        ),
        DropdownButtonFormField<int>(
          value: _idEntidad,
          decoration: const InputDecoration(labelText: 'Tipo de entidad *'),
          isExpanded: true,
          items: _entidades.map((e) => DropdownMenuItem(value: e.id, child: Text(e.nombre))).toList(),
          onChanged: (v) => setState(() => _idEntidad = v),
          validator: (v) => v == null ? 'Campo requerido' : null,
        ),
        TextFormField(
          controller: _entidadOtraCtrl,
          decoration: const InputDecoration(labelText: 'Nombre de la entidad *'),
          textCapitalization: TextCapitalization.words,
          validator: (v) => v == null || v.isEmpty ? 'Campo requerido' : null,
        ),
        Autocomplete<String>(
          optionsBuilder: (value) {
            if (value.text.isEmpty) return _areasLaborales;
            return _areasLaborales.where((a) => a.toLowerCase().contains(value.text.toLowerCase()));
          },
          onSelected: (v) => _areaLaboraCtrl.text = v,
          fieldViewBuilder: (context, controller, focusNode, onSubmitted) {
            // Mantiene sincronizado el controller real (el de Autocomplete es interno).
            controller.text = _areaLaboraCtrl.text;
            controller.addListener(() => _areaLaboraCtrl.text = controller.text);
            return TextFormField(
              controller: controller,
              focusNode: focusNode,
              decoration: const InputDecoration(
                labelText: 'Área donde labora *',
                helperText: 'Escribe para buscar o ingresa una nueva',
              ),
              textCapitalization: TextCapitalization.words,
              validator: (v) => v == null || v.isEmpty ? 'Campo requerido' : null,
            );
          },
        ),
        _BuscadorMaestro(
          label: 'Cargo que desempeña *',
          opciones: _cargos,
          seleccionado: _idCargo,
          onSelected: (id) => setState(() => _idCargo = id),
          validator: () => _idCargo == null ? 'Campo requerido' : null,
          onAgregarNuevo: (nombre) async {
            final creado = await getIt<MaestrosRemoteDataSource>().crearCargo(nombre);
            setState(() {
              _cargos = [..._cargos, creado]..sort((a, b) => a.nombre.compareTo(b.nombre));
            });
            return creado;
          },
        ),
      ],
    );
  }

  Widget _buildConfirmar(PersonaState state) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('DNI: ${_dniCtrl.text}'),
        Text('Nombres: ${_nombresCtrl.text} ${_apPaternoCtrl.text} ${_apMaternoCtrl.text}'),
        Text('Email: ${_emailCtrl.text}'),
        Text('Celular: ${_celularCtrl.text}'),
        const SizedBox(height: 16),
        const Text('Tu contraseña inicial será tu número de DNI.',
            style: TextStyle(fontWeight: FontWeight.bold)),
        if (state is PersonaCreando) const LinearProgressIndicator(),
      ],
    );
  }

  void _registrar(BuildContext context) {
    final persona = PersonaModel(
      idPersona: 0,
      codPersona: '',
      idTipoDocumento: 1,
      numDocumento: _dniCtrl.text,
      nombres: _nombresCtrl.text.toUpperCase(),
      apellidoPaterno: _apPaternoCtrl.text.toUpperCase(),
      apellidoMaterno: _apMaternoCtrl.text.toUpperCase(),
      sexo: _sexo,
      idPaisNacimiento: _idPaisNacimiento!,
      fechaNacimiento: _fechaNacimiento,
      email: _emailCtrl.text,
      celular: _celularCtrl.text,
      idProfesion: _idProfesion,
      validadoPide: 'N',
    );

    final personaData = PersonaDataModel(
      idPersona: 0,
      idPaisLabora: _idPaisLabora!,
      idEntidad: _idEntidad!,
      nombreEntidadOtra: _entidadOtraCtrl.text.toUpperCase(),
      areaLabora: _areaLaboraCtrl.text.toUpperCase(),
      idCargo: _idCargo!,
      nombreCargoOtra: '',
      idDistrito: _idDistritoLabora!.toString(),
    );

    context.read<PersonaCubit>().registrarNuevoParticipante(persona, personaData);
  }

  @override
  void dispose() {
    _dniCtrl.dispose();
    _nombresCtrl.dispose();
    _apPaternoCtrl.dispose();
    _apMaternoCtrl.dispose();
    _emailCtrl.dispose();
    _celularCtrl.dispose();
    _entidadOtraCtrl.dispose();
    _areaLaboraCtrl.dispose();
    super.dispose();
  }
}

// Campo de selección con búsqueda para listas largas (p.ej. Profesiones, ~2000 ítems),
// donde un DropdownButtonFormField normal sería inutilizable.
class _BuscadorMaestro extends FormField<int> {
  _BuscadorMaestro({
    required String label,
    required List<MaestroItem> opciones,
    required int? seleccionado,
    required ValueChanged<int?> onSelected,
    required String? Function() validator,
    Future<MaestroItem> Function(String nombre)? onAgregarNuevo,
  }) : super(
          initialValue: seleccionado,
          validator: (_) => validator(),
          builder: (state) {
            final nombre = opciones.firstWhereOrNull((o) => o.id == seleccionado)?.nombre;
            return InkWell(
              onTap: () async {
                final elegido = await showDialog<MaestroItem>(
                  context: state.context,
                  builder: (dialogContext) =>
                      _DialogoBusqueda(opciones: opciones, onAgregarNuevo: onAgregarNuevo),
                );
                if (elegido != null) {
                  onSelected(elegido.id);
                  state.didChange(elegido.id);
                }
              },
              child: InputDecorator(
                decoration: InputDecoration(labelText: label, errorText: state.errorText),
                child: Text(nombre ?? 'Seleccionar...', overflow: TextOverflow.ellipsis),
              ),
            );
          },
        );
}

class _DialogoBusqueda extends StatefulWidget {
  final List<MaestroItem> opciones;
  final Future<MaestroItem> Function(String nombre)? onAgregarNuevo;
  const _DialogoBusqueda({required this.opciones, this.onAgregarNuevo});

  @override
  State<_DialogoBusqueda> createState() => _DialogoBusquedaState();
}

class _DialogoBusquedaState extends State<_DialogoBusqueda> {
  String _filtro = '';
  bool _agregando = false;

  Future<void> _agregarNuevo() async {
    if (widget.onAgregarNuevo == null || _filtro.trim().isEmpty || _agregando) return;
    setState(() => _agregando = true);
    try {
      final creado = await widget.onAgregarNuevo!(_filtro.trim().toUpperCase());
      if (mounted) Navigator.pop(context, creado);
    } catch (e) {
      if (mounted) {
        setState(() => _agregando = false);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('No se pudo agregar. Intenta de nuevo.'), backgroundColor: Colors.red),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final filtrados = _filtro.isEmpty
        ? widget.opciones
        : widget.opciones
            .where((o) => o.nombre.toLowerCase().contains(_filtro.toLowerCase()))
            .toList();
    final sinCoincidenciaExacta = _filtro.trim().isNotEmpty &&
        !widget.opciones.any((o) => o.nombre.toLowerCase() == _filtro.trim().toLowerCase());
    return Dialog(
      child: SizedBox(
        width: double.maxFinite,
        height: 500,
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.all(12),
              child: TextField(
                autofocus: true,
                decoration: const InputDecoration(
                  prefixIcon: Icon(Icons.search),
                  hintText: 'Buscar...',
                  border: OutlineInputBorder(),
                ),
                onChanged: (v) => setState(() => _filtro = v),
              ),
            ),
            if (widget.onAgregarNuevo != null && sinCoincidenciaExacta)
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 12),
                child: ListTile(
                  leading: _agregando
                      ? const SizedBox(width: 24, height: 24, child: CircularProgressIndicator(strokeWidth: 2))
                      : const Icon(Icons.add_circle, color: Colors.green),
                  title: Text('Agregar "${_filtro.trim()}"'),
                  onTap: _agregando ? null : _agregarNuevo,
                ),
              ),
            const Divider(height: 1),
            Expanded(
              child: ListView.builder(
                itemCount: filtrados.length,
                itemBuilder: (_, i) => ListTile(
                  title: Text(filtrados[i].nombre),
                  onTap: () => Navigator.pop(context, filtrados[i]),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

extension _FirstWhereOrNull<T> on List<T> {
  T? firstWhereOrNull(bool Function(T) test) {
    for (final e in this) {
      if (test(e)) return e;
    }
    return null;
  }
}

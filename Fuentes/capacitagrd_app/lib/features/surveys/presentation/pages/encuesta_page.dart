import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../../../core/constants/app_constants.dart';
import '../../../../core/di/injection.dart';
import '../../../../shared/widgets/loading_widget.dart';
import '../../../../shared/widgets/error_widget.dart';
import '../../data/models/encuesta_model.dart';
import '../bloc/encuesta_cubit.dart';

class EncuestaPage extends StatefulWidget {
  final int idEncuesta;
  final int idEvento;
  const EncuestaPage({super.key, required this.idEncuesta, required this.idEvento});

  @override
  State<EncuestaPage> createState() => _EncuestaPageState();
}

class _EncuestaPageState extends State<EncuestaPage> {
  int? _idPersona;

  @override
  void initState() {
    super.initState();
    _init();
  }

  Future<void> _init() async {
    final idStr = await getIt<FlutterSecureStorage>().read(key: AppConstants.personaIdKey);
    setState(() => _idPersona = int.tryParse(idStr ?? ''));
    if (mounted) context.read<EncuestaCubit>().cargarEncuesta(widget.idEncuesta);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Encuesta de Satisfacción')),
      body: BlocConsumer<EncuestaCubit, EncuestaState>(
        listener: (context, state) {
          if (state is EncuestaCompletada) {
            showDialog(
              context: context,
              barrierDismissible: false,
              builder: (dialogContext) => AlertDialog(
                title: const Text('Encuesta Guardada'),
                content: const Text('Tus respuestas han sido guardadas. Serán enviadas cuando haya conexión.'),
                actions: [FilledButton(onPressed: () => Navigator.of(dialogContext).pop(), child: const Text('OK'))],
              ),
            );
          }
          if (state is EncuestaError) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.red),
            );
          }
        },
        builder: (context, state) {
          if (state is EncuestaLoading) return const LoadingWidget(mensaje: 'Cargando encuesta...');
          if (state is EncuestaError) {
            return AppErrorWidget(
              message: state.message,
              onRetry: () => context.read<EncuestaCubit>().cargarEncuesta(widget.idEncuesta),
            );
          }
          if (state is EncuestaLoaded) {
            return Column(
              children: [
                Expanded(
                  child: ListView.builder(
                    padding: const EdgeInsets.all(16),
                    itemCount: state.encuesta.preguntas.length,
                    itemBuilder: (_, i) {
                      final pregunta = state.encuesta.preguntas[i];
                      return _PreguntaWidget(
                        pregunta: pregunta,
                        respuesta: state.respuestas[pregunta.idRespuesta],
                        onChanged: (r) => context.read<EncuestaCubit>().responder(pregunta.idRespuesta, r),
                      );
                    },
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.all(16),
                  child: SizedBox(
                    width: double.infinity,
                    child: FilledButton(
                      onPressed: state.completa
                          ? () => context.read<EncuestaCubit>().guardarRespuestas(
                                idPersona: _idPersona ?? 0,
                                idEvento: widget.idEvento,
                              )
                          : null,
                      child: const Text('Enviar Encuesta'),
                    ),
                  ),
                ),
              ],
            );
          }
          return const SizedBox();
        },
      ),
    );
  }
}

class _PreguntaWidget extends StatelessWidget {
  final EncuestaRespuestaModel pregunta;
  final String? respuesta;
  final ValueChanged<String> onChanged;

  const _PreguntaWidget({required this.pregunta, this.respuesta, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    final opciones = pregunta.opcionesLista;

    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(pregunta.nombre, style: Theme.of(context).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.bold)),
            const SizedBox(height: 12),
            if (opciones.isNotEmpty)
              ...opciones.map((op) => RadioListTile<String>(
                    value: op,
                    groupValue: respuesta,
                    onChanged: (v) => onChanged(v ?? ''),
                    title: Text(op),
                    contentPadding: EdgeInsets.zero,
                  ))
            else
              TextField(
                decoration: const InputDecoration(hintText: 'Escribe tu respuesta...', border: OutlineInputBorder()),
                maxLines: 3,
                onChanged: onChanged,
              ),
          ],
        ),
      ),
    );
  }
}

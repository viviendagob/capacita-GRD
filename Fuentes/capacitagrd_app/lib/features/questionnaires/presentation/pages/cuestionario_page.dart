import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../../../core/constants/app_constants.dart';
import '../../../../core/di/injection.dart';
import '../../../../shared/widgets/loading_widget.dart';
import '../../../../shared/widgets/error_widget.dart';
import '../../data/models/cuestionario_model.dart';
import '../bloc/cuestionario_cubit.dart';

class CuestionarioPage extends StatefulWidget {
  final int idCuestionario;
  final int idEvento;
  const CuestionarioPage({super.key, required this.idCuestionario, required this.idEvento});

  @override
  State<CuestionarioPage> createState() => _CuestionarioPageState();
}

class _CuestionarioPageState extends State<CuestionarioPage> {
  int? _idPersona;

  @override
  void initState() {
    super.initState();
    _init();
  }

  Future<void> _init() async {
    final idStr = await getIt<FlutterSecureStorage>().read(key: AppConstants.personaIdKey);
    setState(() => _idPersona = int.tryParse(idStr ?? ''));
    if (mounted) context.read<CuestionarioCubit>().cargarCuestionario(widget.idCuestionario);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Cuestionario')),
      body: BlocConsumer<CuestionarioCubit, CuestionarioState>(
        listener: (context, state) {
          if (state is CuestionarioCompletado) {
            _mostrarResultado(context, state.resultado);
          }
          if (state is CuestionarioError) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.red),
            );
          }
        },
        builder: (context, state) {
          if (state is CuestionarioLoading) return const LoadingWidget(mensaje: 'Cargando cuestionario...');
          if (state is CuestionarioError) {
            return AppErrorWidget(
              message: state.message,
              onRetry: () => context.read<CuestionarioCubit>().cargarCuestionario(widget.idCuestionario),
            );
          }
          if (state is CuestionarioLoaded) {
            return Column(
              children: [
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.all(12),
                  color: Theme.of(context).colorScheme.primaryContainer,
                  child: Text(
                    '${state.respuestas.length}/${state.cuestionario.preguntas.length} respondidas',
                    style: TextStyle(color: Theme.of(context).colorScheme.onPrimaryContainer),
                    textAlign: TextAlign.center,
                  ),
                ),
                Expanded(
                  child: ListView.builder(
                    padding: const EdgeInsets.all(16),
                    itemCount: state.cuestionario.preguntas.length,
                    itemBuilder: (_, i) {
                      final p = state.cuestionario.preguntas[i];
                      return _PreguntaCard(
                        numero: i + 1,
                        pregunta: p,
                        respuestaSeleccionada: state.respuestas[p.idCuestionarioPregunta],
                        onChanged: (idResp) =>
                            context.read<CuestionarioCubit>().seleccionarRespuesta(p.idCuestionarioPregunta, idResp),
                      );
                    },
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.all(16),
                  child: SizedBox(
                    width: double.infinity,
                    child: FilledButton(
                      onPressed: state.completo
                          ? () => context.read<CuestionarioCubit>().enviarCuestionario(
                                idPersona: _idPersona ?? 0,
                                idEvento: widget.idEvento,
                              )
                          : null,
                      child: const Text('Finalizar Cuestionario'),
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

  void _mostrarResultado(BuildContext context, ResultadoCuestionario resultado) {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (dialogContext) => AlertDialog(
        title: Text(resultado.aprobado ? '¡Aprobado!' : 'Resultado'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(resultado.aprobado ? Icons.check_circle : Icons.cancel,
                size: 64, color: resultado.aprobado ? Colors.green : Colors.red),
            const SizedBox(height: 16),
            Text('${resultado.porcentaje.toStringAsFixed(0)}%',
                style: const TextStyle(fontSize: 48, fontWeight: FontWeight.bold)),
            Text('${resultado.puntajeObtenido} / ${resultado.puntajeTotal} puntos'),
          ],
        ),
        actions: [
          FilledButton(
            onPressed: () => Navigator.of(dialogContext).pop(),
            child: const Text('Finalizar'),
          ),
        ],
      ),
    );
  }
}

class _PreguntaCard extends StatelessWidget {
  final int numero;
  final CuestionarioPreguntaModel pregunta;
  final int? respuestaSeleccionada;
  final ValueChanged<int> onChanged;

  const _PreguntaCard({
    required this.numero,
    required this.pregunta,
    this.respuestaSeleccionada,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  radius: 14,
                  backgroundColor: Theme.of(context).colorScheme.primary,
                  child: Text('$numero', style: const TextStyle(color: Colors.white, fontSize: 12)),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(pregunta.nombre,
                      style: Theme.of(context).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.bold)),
                ),
                Text('${pregunta.peso} pts', style: Theme.of(context).textTheme.bodySmall),
              ],
            ),
            const SizedBox(height: 12),
            ...pregunta.respuestas.map((r) => RadioListTile<int>(
                  value: r.idPreguntaRespuesta,
                  groupValue: respuestaSeleccionada,
                  onChanged: (v) => onChanged(v ?? 0),
                  title: Text(r.nombre),
                  contentPadding: EdgeInsets.zero,
                )),
          ],
        ),
      ),
    );
  }
}

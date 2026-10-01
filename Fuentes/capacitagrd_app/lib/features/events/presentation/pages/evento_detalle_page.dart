import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import '../../../../core/constants/app_constants.dart';
import '../../../../core/di/injection.dart';
import '../../../../shared/widgets/loading_widget.dart';
import '../../../../shared/widgets/error_widget.dart';
import '../../domain/repositories/eventos_repository.dart';
import '../bloc/eventos_cubit.dart';

class EventoDetallePage extends StatefulWidget {
  final int idEvento;
  const EventoDetallePage({super.key, required this.idEvento});

  @override
  State<EventoDetallePage> createState() => _EventoDetallePageState();
}

class _EventoDetallePageState extends State<EventoDetallePage> {
  int? _idPersona;
  int? _idPersonaData;
  bool? _inscrito;
  String? _miQrUrl;
  bool _esAdmin = false;

  @override
  void initState() {
    super.initState();
    context.read<EventosCubit>().loadEvento(widget.idEvento);
    _cargarInscripcion();
    _cargarRol();
  }

  Future<void> _cargarRol() async {
    final rol = await getIt<FlutterSecureStorage>().read(key: AppConstants.rolKey);
    if (!mounted) return;
    setState(() => _esAdmin = rol == 'ADMIN');
  }

  Future<void> _cargarInscripcion() async {
    final storage = getIt<FlutterSecureStorage>();
    final idPersonaStr = await storage.read(key: AppConstants.personaIdKey);
    final idPersonaDataStr = await storage.read(key: AppConstants.personaDataIdKey);
    final idPersona = int.tryParse(idPersonaStr ?? '');
    final result = idPersona == null
        ? null
        : await getIt<EventosRepository>().verificarInscripcion(widget.idEvento, idPersona);
    if (!mounted) return;
    final participante = result?.fold((_) => null, (v) => v);
    setState(() {
      _idPersona = idPersona;
      _idPersonaData = int.tryParse(idPersonaDataStr ?? '');
      _inscrito = participante != null;
      _miQrUrl = participante?.qrUrl;
    });
  }

  void _mostrarQr(BuildContext context, String qrUrl) {
    showDialog(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Tu código QR'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Image.network(qrUrl, width: 220, height: 220,
                errorBuilder: (_, __, ___) => const Icon(Icons.qr_code, size: 220)),
            const SizedBox(height: 12),
            const Text(
              'Este QR es tu entrada personal a este evento. Muéstralo al staff para que registre '
              'tu asistencia (presencial o virtual).',
              textAlign: TextAlign.center,
            ),
          ],
        ),
        actions: [
          FilledButton(onPressed: () => Navigator.pop(dialogContext), child: const Text('Listo')),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Detalle del Evento')),
      body: BlocConsumer<EventosCubit, EventosState>(
        listener: (context, state) {
          if (state is EventoInscritoExito) {
            setState(() {
              _inscrito = true;
              _miQrUrl = state.participante.qrUrl;
            });
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('¡Te inscribiste correctamente!'), backgroundColor: Colors.green),
            );
            if (state.participante.qrUrl != null) {
              _mostrarQr(context, state.participante.qrUrl!);
            }
          }
          if (state is EventosError) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.red),
            );
          }
        },
        builder: (context, state) {
          if (state is EventoDetailLoading || state is EventosLoading) {
            return const LoadingWidget(mensaje: 'Cargando evento...');
          }
          if (state is EventosError) {
            return AppErrorWidget(
              message: state.message,
              onRetry: () => context.read<EventosCubit>().loadEvento(widget.idEvento),
            );
          }
          if (state is EventoDetailLoaded) {
            final e = state.evento;
            final fmt = DateFormat('dd/MM/yyyy');
            return SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  if (e.banner != null)
                    ClipRRect(
                      borderRadius: BorderRadius.circular(12),
                      child: Image.network(e.banner!, height: 200, width: double.infinity, fit: BoxFit.cover,
                          errorBuilder: (_, __, ___) => const SizedBox()),
                    ),
                  const SizedBox(height: 16),
                  Text(e.nombre, style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.bold)),
                  const SizedBox(height: 8),
                  _InfoRow(icon: Icons.event, text: '${fmt.format(e.fechaInicio)} — ${fmt.format(e.fechaFin)}'),
                  if (e.horaInicio != null) _InfoRow(icon: Icons.schedule, text: '${e.horaInicio} - ${e.horaFin ?? ''}'),
                  _InfoRow(icon: Icons.location_on, text: e.nombreLugar),
                  _InfoRow(icon: Icons.group, text: '${e.numParticipantes} participantes'),
                  _InfoRow(icon: Icons.category, text: e.tipoEvento?.nombre ?? ''),
                  _InfoRow(icon: Icons.video_camera_front, text: e.modalidad?.nombre ?? ''),
                  if (e.descripcion.isNotEmpty) ...[
                    const SizedBox(height: 16),
                    Text('Descripción', style: Theme.of(context).textTheme.titleMedium),
                    const SizedBox(height: 4),
                    Text(e.descripcion),
                  ],
                  if (e.fechas.isNotEmpty) ...[
                    const SizedBox(height: 16),
                    Text('Sesiones', style: Theme.of(context).textTheme.titleMedium),
                    ...e.fechas.map((f) => ListTile(
                          leading: const Icon(Icons.event_note),
                          title: Text(fmt.format(f.fecha)),
                          subtitle: Text('${f.horaInicio ?? ''} - ${f.horaFin ?? ''}'),
                          trailing: f.esEncuesta == 'S'
                              ? const Chip(label: Text('Encuesta'))
                              : f.esCuestionario == 'S'
                                  ? const Chip(label: Text('Cuestionario'))
                                  : null,
                        )),
                  ],
                  const SizedBox(height: 24),
                  if (_inscrito == null)
                    const Center(child: CircularProgressIndicator())
                  else if (_inscrito == false)
                    SizedBox(
                      width: double.infinity,
                      child: FilledButton.icon(
                        onPressed: state is EventoInscribiendo || _idPersona == null || _idPersonaData == null
                            ? null
                            : () => context.read<EventosCubit>().inscribirse(
                                  idEvento: widget.idEvento,
                                  idPersona: _idPersona!,
                                  idPersonaData: _idPersonaData!,
                                  idModalidad: e.idModalidad,
                                ),
                        icon: state is EventoInscribiendo
                            ? const SizedBox(
                                width: 16, height: 16, child: CircularProgressIndicator(strokeWidth: 2))
                            : const Icon(Icons.how_to_reg),
                        label: const Text('Inscribirme'),
                      ),
                    )
                  else ...[
                    if (_miQrUrl != null) ...[
                      SizedBox(
                        width: double.infinity,
                        child: FilledButton.icon(
                          onPressed: () => _mostrarQr(context, _miQrUrl!),
                          icon: const Icon(Icons.qr_code),
                          label: const Text('Mi código QR'),
                        ),
                      ),
                      const SizedBox(height: 8),
                    ],
                    if (e.idEncuesta != null) ...[
                      SizedBox(
                        width: double.infinity,
                        child: OutlinedButton.icon(
                          onPressed: () => context.push('/encuesta/${e.idEncuesta}/${widget.idEvento}'),
                          icon: const Icon(Icons.poll),
                          label: const Text('Responder Encuesta'),
                        ),
                      ),
                      const SizedBox(height: 8),
                    ],
                    if (e.idCuestionario != null) ...[
                      SizedBox(
                        width: double.infinity,
                        child: OutlinedButton.icon(
                          onPressed: () => context.push('/cuestionario/${e.idCuestionario}/${widget.idEvento}'),
                          icon: const Icon(Icons.quiz),
                          label: const Text('Rendir Cuestionario'),
                        ),
                      ),
                      const SizedBox(height: 8),
                    ],
                  ],
                  if (_esAdmin) ...[
                    const Divider(height: 32),
                    Text('Panel de staff', style: Theme.of(context).textTheme.titleSmall?.copyWith(color: Colors.grey)),
                    const SizedBox(height: 8),
                    SizedBox(
                      width: double.infinity,
                      child: OutlinedButton.icon(
                        onPressed: () => context.push('/validar-asistencia/${widget.idEvento}'),
                        icon: const Icon(Icons.qr_code_scanner),
                        label: const Text('Validar Asistencias'),
                      ),
                    ),
                  ],
                ],
              ),
            );
          }
          return const SizedBox();
        },
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  final IconData icon;
  final String text;

  const _InfoRow({required this.icon, required this.text});

  @override
  Widget build(BuildContext context) {
    if (text.isEmpty) return const SizedBox();
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        children: [
          Icon(icon, size: 18, color: Theme.of(context).colorScheme.primary),
          const SizedBox(width: 8),
          Expanded(child: Text(text, style: Theme.of(context).textTheme.bodyMedium)),
        ],
      ),
    );
  }
}

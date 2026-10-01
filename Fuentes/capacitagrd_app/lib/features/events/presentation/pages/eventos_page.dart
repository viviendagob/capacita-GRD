import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import '../../../../core/constants/app_constants.dart';
import '../../../../core/di/injection.dart';
import '../../../../shared/widgets/loading_widget.dart';
import '../../../../shared/widgets/error_widget.dart';
import '../../../../shared/widgets/empty_widget.dart';
import '../../data/models/evento_model.dart';
import '../bloc/eventos_cubit.dart';

class EventosPage extends StatefulWidget {
  const EventosPage({super.key});

  @override
  State<EventosPage> createState() => _EventosPageState();
}

class _EventosPageState extends State<EventosPage> {
  @override
  void initState() {
    super.initState();
    _cargar();
  }

  Future<void> _cargar() async {
    final storage = getIt<FlutterSecureStorage>();
    final idStr = await storage.read(key: AppConstants.personaIdKey);
    final idPersona = int.tryParse(idStr ?? '') ?? 0;
    if (mounted) {
      context.read<EventosCubit>().loadMisEventos(idPersona);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Mis Eventos'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _cargar,
          ),
        ],
      ),
      body: BlocBuilder<EventosCubit, EventosState>(
        builder: (context, state) {
          if (state is EventosLoading) return const LoadingWidget(mensaje: 'Cargando eventos...');
          if (state is EventosError) {
            return AppErrorWidget(message: state.message, onRetry: _cargar);
          }
          if (state is EventosLoaded) {
            if (state.eventos.isEmpty) {
              return const EmptyWidget(
                mensaje: 'No tienes eventos inscritos aún.',
                icon: Icons.event_busy,
              );
            }
            return RefreshIndicator(
              onRefresh: _cargar,
              child: ListView.builder(
                padding: const EdgeInsets.all(16),
                itemCount: state.eventos.length,
                itemBuilder: (_, i) => _EventoCard(participante: state.eventos[i]),
              ),
            );
          }
          return const SizedBox();
        },
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => context.push('/eventos-disponibles'),
        icon: const Icon(Icons.search),
        label: const Text('Eventos disponibles'),
      ),
    );
  }
}

class _EventoCard extends StatelessWidget {
  final EventoParticipanteModel participante;

  const _EventoCard({required this.participante});

  @override
  Widget build(BuildContext context) {
    final evento = participante.evento;
    final colorEstado = evento?.isActivo == true ? Colors.green : Colors.grey;
    final fmt = DateFormat('dd/MM/yyyy');

    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: InkWell(
        onTap: () => context.push('/eventos/${participante.idEvento}'),
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      evento?.nombre ?? 'Evento #${participante.idEvento}',
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.bold),
                    ),
                  ),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: colorEstado.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(12),
                      border: Border.all(color: colorEstado),
                    ),
                    child: Text(
                      evento?.estado?.nombre ?? 'Activo',
                      style: TextStyle(color: colorEstado, fontSize: 11),
                    ),
                  ),
                ],
              ),
              if (evento != null) ...[
                const SizedBox(height: 8),
                Row(children: [
                  const Icon(Icons.calendar_today, size: 14),
                  const SizedBox(width: 4),
                  Text('${fmt.format(evento.fechaInicio)} - ${fmt.format(evento.fechaFin)}',
                      style: Theme.of(context).textTheme.bodySmall),
                ]),
                const SizedBox(height: 4),
                Row(children: [
                  const Icon(Icons.location_on, size: 14),
                  const SizedBox(width: 4),
                  Expanded(
                      child: Text(evento.nombreLugar, style: Theme.of(context).textTheme.bodySmall,
                          maxLines: 1, overflow: TextOverflow.ellipsis)),
                ]),
                const SizedBox(height: 4),
                Row(children: [
                  const Icon(Icons.video_camera_front, size: 14),
                  const SizedBox(width: 4),
                  Text(evento.modalidad?.nombre ?? (evento.isVirtual ? 'Virtual' : 'Presencial'),
                      style: Theme.of(context).textTheme.bodySmall),
                ]),
              ],
              const SizedBox(height: 8),
              Text('Inscrito: ${DateFormat('dd/MM/yyyy HH:mm').format(participante.fechaReg)}',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Colors.grey)),
            ],
          ),
        ),
      ),
    );
  }
}

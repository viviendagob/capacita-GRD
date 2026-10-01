import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import '../../../../shared/widgets/loading_widget.dart';
import '../../../../shared/widgets/error_widget.dart';
import '../../../../shared/widgets/empty_widget.dart';
import '../bloc/eventos_cubit.dart';

class EventosDisponiblesPage extends StatefulWidget {
  const EventosDisponiblesPage({super.key});

  @override
  State<EventosDisponiblesPage> createState() => _EventosDisponiblesPageState();
}

class _EventosDisponiblesPageState extends State<EventosDisponiblesPage> {
  @override
  void initState() {
    super.initState();
    context.read<EventosCubit>().loadEventosDisponibles();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Eventos Disponibles')),
      body: BlocBuilder<EventosCubit, EventosState>(
        builder: (context, state) {
          if (state is EventosLoading) return const LoadingWidget(mensaje: 'Cargando eventos...');
          if (state is EventosError) {
            return AppErrorWidget(
              message: state.message,
              onRetry: () => context.read<EventosCubit>().loadEventosDisponibles(),
            );
          }
          if (state is EventosDisponiblesLoaded) {
            if (state.eventos.isEmpty) {
              return const EmptyWidget(mensaje: 'No hay eventos disponibles por el momento.', icon: Icons.event_busy);
            }
            final fmt = DateFormat('dd/MM/yyyy');
            return RefreshIndicator(
              onRefresh: () => context.read<EventosCubit>().loadEventosDisponibles(),
              child: ListView.builder(
                padding: const EdgeInsets.all(16),
                itemCount: state.eventos.length,
                itemBuilder: (_, i) {
                  final e = state.eventos[i];
                  return Card(
                    margin: const EdgeInsets.only(bottom: 12),
                    child: InkWell(
                      onTap: () => context.push('/eventos/${e.idEvento}'),
                      borderRadius: BorderRadius.circular(12),
                      child: Padding(
                        padding: const EdgeInsets.all(16),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(e.nombre,
                                style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.bold)),
                            const SizedBox(height: 8),
                            Row(children: [
                              const Icon(Icons.calendar_today, size: 14),
                              const SizedBox(width: 4),
                              Text('${fmt.format(e.fechaInicio)} - ${fmt.format(e.fechaFin)}',
                                  style: Theme.of(context).textTheme.bodySmall),
                            ]),
                            const SizedBox(height: 4),
                            Row(children: [
                              const Icon(Icons.location_on, size: 14),
                              const SizedBox(width: 4),
                              Expanded(
                                  child: Text(e.nombreLugar,
                                      style: Theme.of(context).textTheme.bodySmall,
                                      maxLines: 1,
                                      overflow: TextOverflow.ellipsis)),
                            ]),
                            const SizedBox(height: 4),
                            Row(children: [
                              const Icon(Icons.video_camera_front, size: 14),
                              const SizedBox(width: 4),
                              Text(e.modalidad?.nombre ?? '', style: Theme.of(context).textTheme.bodySmall),
                            ]),
                          ],
                        ),
                      ),
                    ),
                  );
                },
              ),
            );
          }
          return const SizedBox();
        },
      ),
    );
  }
}

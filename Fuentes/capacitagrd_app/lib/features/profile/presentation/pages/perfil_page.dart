import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import '../../../../shared/widgets/loading_widget.dart';
import '../../../../shared/widgets/error_widget.dart';
import '../../../auth/presentation/bloc/auth_cubit.dart';
import '../bloc/persona_cubit.dart';

class PerfilPage extends StatefulWidget {
  const PerfilPage({super.key});

  @override
  State<PerfilPage> createState() => _PerfilPageState();
}

class _PerfilPageState extends State<PerfilPage> {
  @override
  void initState() {
    super.initState();
    context.read<PersonaCubit>().cargarPerfil();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Mi Perfil'),
        actions: [
          IconButton(
            icon: const Icon(Icons.edit),
            onPressed: () => context.push('/perfil/editar'),
          ),
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () => _confirmarLogout(context),
          ),
        ],
      ),
      body: BlocBuilder<PersonaCubit, PersonaState>(
        builder: (context, state) {
          if (state is PersonaLoading) return const LoadingWidget(mensaje: 'Cargando perfil...');
          if (state is PersonaError) {
            return AppErrorWidget(
              message: state.message,
              onRetry: () => context.read<PersonaCubit>().cargarPerfil(),
            );
          }
          if (state is PersonaLoaded) {
            final p = state.persona;
            final fmt = DateFormat('dd/MM/yyyy');
            return SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                children: [
                  CircleAvatar(
                    radius: 50,
                    backgroundColor: Theme.of(context).colorScheme.primaryContainer,
                    child: Text(
                      p.nombres.isNotEmpty ? p.nombres[0] : '?',
                      style: TextStyle(fontSize: 36, color: Theme.of(context).colorScheme.primary),
                    ),
                  ),
                  const SizedBox(height: 12),
                  Text(p.nombreCompleto,
                      style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.bold),
                      textAlign: TextAlign.center),
                  Text(p.numDocumento, style: Theme.of(context).textTheme.titleMedium?.copyWith(color: Colors.grey)),
                  if (p.validado) ...[
                    const SizedBox(height: 4),
                    const Chip(
                      label: Text('Validado PIDE'),
                      avatar: Icon(Icons.verified, size: 16),
                    ),
                  ],
                  const SizedBox(height: 24),
                  _InfoCard(
                    title: 'Datos Personales',
                    children: [
                      _RowInfo('Email', p.email ?? 'No registrado'),
                      _RowInfo('Celular', p.celular ?? 'No registrado'),
                      _RowInfo('Sexo', p.sexo == 'M' ? 'Masculino' : 'Femenino'),
                      if (p.fechaNacimiento != null) _RowInfo('Fecha de Nacimiento', fmt.format(p.fechaNacimiento!)),
                    ],
                  ),
                  if (state.personaData != null) ...[
                    const SizedBox(height: 16),
                    _InfoCard(
                      title: 'Datos Laborales',
                      children: [
                        _RowInfo('Área', state.personaData!.areaLabora),
                      ],
                    ),
                  ],
                  const SizedBox(height: 24),
                  OutlinedButton.icon(
                    onPressed: () => context.push('/certificados'),
                    icon: const Icon(Icons.card_membership),
                    label: const Text('Mis Certificados'),
                  ),
                ],
              ),
            );
          }
          return const SizedBox();
        },
      ),
    );
  }

  void _confirmarLogout(BuildContext context) {
    showDialog(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Cerrar Sesión'),
        content: const Text('¿Deseas cerrar sesión?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(dialogContext), child: const Text('Cancelar')),
          FilledButton(
            onPressed: () {
              Navigator.pop(dialogContext);
              // OJO: getIt<AuthCubit>() crearía una instancia nueva (está registrado como
              // factory) desconectada de la que escucha el router (ver app.dart), y el
              // logout nunca redirigiría a /login. Debe ser la misma instancia del árbol.
              context.read<AuthCubit>().logout();
            },
            child: const Text('Salir'),
          ),
        ],
      ),
    );
  }
}

class _InfoCard extends StatelessWidget {
  final String title;
  final List<Widget> children;
  const _InfoCard({required this.title, required this.children});

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(title, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.bold)),
            const Divider(),
            ...children,
          ],
        ),
      ),
    );
  }
}

class _RowInfo extends StatelessWidget {
  final String label;
  final String value;
  const _RowInfo(this.label, this.value);

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: Theme.of(context).textTheme.bodyMedium?.copyWith(color: Colors.grey)),
          Flexible(child: Text(value, textAlign: TextAlign.right, style: Theme.of(context).textTheme.bodyMedium)),
        ],
      ),
    );
  }
}

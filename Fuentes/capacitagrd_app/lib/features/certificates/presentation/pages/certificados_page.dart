import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:intl/intl.dart';
import 'package:share_plus/share_plus.dart';
import '../../../../core/constants/app_constants.dart';
import '../../../../core/di/injection.dart';
import '../../../../shared/widgets/loading_widget.dart';
import '../../../../shared/widgets/error_widget.dart';
import '../../../../shared/widgets/empty_widget.dart';
import '../bloc/certificado_cubit.dart';

class CertificadosPage extends StatefulWidget {
  const CertificadosPage({super.key});

  @override
  State<CertificadosPage> createState() => _CertificadosPageState();
}

class _CertificadosPageState extends State<CertificadosPage> {
  int? _idPersona;

  @override
  void initState() {
    super.initState();
    _cargar();
  }

  Future<void> _cargar() async {
    final idStr = await getIt<FlutterSecureStorage>().read(key: AppConstants.personaIdKey);
    setState(() => _idPersona = int.tryParse(idStr ?? ''));
    if (mounted && _idPersona != null) {
      context.read<CertificadoCubit>().cargarMisCertificados(_idPersona!);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Mis Certificados')),
      body: BlocConsumer<CertificadoCubit, CertificadoState>(
        listener: (context, state) {
          if (state is CertificadoUrlObtenido) {
            Share.share(state.url, subject: 'Certificado CAPACITA-GRD');
          }
          if (state is CertificadoError) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.red),
            );
          }
        },
        builder: (context, state) {
          if (state is CertificadoLoading) return const LoadingWidget(mensaje: 'Cargando certificados...');
          if (state is CertificadoError) {
            return AppErrorWidget(message: state.message, onRetry: _cargar);
          }
          if (state is CertificadosLoaded) {
            if (state.certificados.isEmpty) {
              return const EmptyWidget(
                mensaje: 'Aún no tienes certificados.\nCompleta un evento para obtener tu certificado.',
                icon: Icons.card_membership_outlined,
              );
            }
            return ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: state.certificados.length,
              itemBuilder: (_, i) {
                final cert = state.certificados[i];
                return Card(
                  margin: const EdgeInsets.only(bottom: 12),
                  child: ListTile(
                    leading: const CircleAvatar(
                      backgroundColor: Color(0xFF003865),
                      child: Icon(Icons.card_membership, color: Colors.white),
                    ),
                    title: Text(cert.nombreEvento, style: const TextStyle(fontWeight: FontWeight.bold)),
                    subtitle: Text(
                        'Emitido: ${DateFormat('dd/MM/yyyy').format(cert.fechaEmision)}'),
                    trailing: IconButton(
                      icon: state is CertificadoDescargando
                          ? const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(strokeWidth: 2))
                          : const Icon(Icons.download),
                      onPressed: state is CertificadoDescargando
                          ? null
                          : () => context
                              .read<CertificadoCubit>()
                              .descargarCertificado(cert.idEvento, _idPersona ?? 0),
                    ),
                  ),
                );
              },
            );
          }
          return const SizedBox();
        },
      ),
    );
  }
}

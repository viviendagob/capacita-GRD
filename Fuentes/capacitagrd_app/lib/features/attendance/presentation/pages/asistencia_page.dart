import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import '../../../../shared/widgets/loading_widget.dart';
import '../bloc/asistencia_cubit.dart';

// Pantalla de STAFF (rol ADMIN): escanea el QR personal de cada participante para
// validar su asistencia al evento (presencial o virtual). No es el participante
// escaneando su propio QR.
class AsistenciaPage extends StatefulWidget {
  final int idEvento;
  const AsistenciaPage({super.key, required this.idEvento});

  @override
  State<AsistenciaPage> createState() => _AsistenciaPageState();
}

class _AsistenciaPageState extends State<AsistenciaPage> {
  bool _scanning = false;
  bool _processed = false;
  final _scannerCtrl = MobileScannerController();

  void _onQRDetected(BarcodeCapture capture) {
    if (_processed) return;
    final code = capture.barcodes.first.rawValue;
    if (code == null) return;
    setState(() => _processed = true);
    _scannerCtrl.stop();

    context.read<AsistenciaCubit>().validarAsistenciaPorQR(idEvento: widget.idEvento, qrData: code);
  }

  void _reintentar() {
    setState(() => _processed = false);
    _scannerCtrl.start();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Validar Asistencias')),
      body: BlocConsumer<AsistenciaCubit, AsistenciaState>(
        listener: (context, state) {
          if (state is AsistenciaRegistradaExito) {
            showDialog(
              context: context,
              barrierDismissible: false,
              builder: (dialogContext) => AlertDialog(
                title: const Text('¡Asistencia validada!'),
                content: Text('Se registró la asistencia de la persona #${state.asistencia.idPersona}.'),
                actions: [
                  FilledButton(
                    onPressed: () {
                      Navigator.of(dialogContext).pop();
                      _reintentar();
                    },
                    child: const Text('Escanear siguiente'),
                  ),
                ],
              ),
            );
          }
          if (state is AsistenciaError) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.red),
            );
            _reintentar();
          }
        },
        builder: (context, state) {
          if (state is AsistenciaRegistrando) return const LoadingWidget(mensaje: 'Validando asistencia...');

          return Column(
            children: [
              Expanded(
                child: _scanning
                    ? MobileScanner(
                        controller: _scannerCtrl,
                        onDetect: _onQRDetected,
                      )
                    : Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            const Icon(Icons.qr_code_scanner, size: 100, color: Color(0xFF003865)),
                            const SizedBox(height: 24),
                            const Text(
                              'Escanea el QR personal de cada\nparticipante para validar su asistencia',
                              textAlign: TextAlign.center,
                              style: TextStyle(fontSize: 16),
                            ),
                            const SizedBox(height: 32),
                            FilledButton.icon(
                              onPressed: () => setState(() {
                                _scanning = true;
                                _processed = false;
                              }),
                              icon: const Icon(Icons.camera_alt),
                              label: const Text('Escanear QR'),
                            ),
                          ],
                        ),
                      ),
              ),
              if (_scanning)
                Padding(
                  padding: const EdgeInsets.all(16),
                  child: TextButton(
                    onPressed: () {
                      _scannerCtrl.stop();
                      setState(() => _scanning = false);
                    },
                    child: const Text('Cancelar'),
                  ),
                ),
            ],
          );
        },
      ),
    );
  }

  @override
  void dispose() {
    _scannerCtrl.dispose();
    super.dispose();
  }
}

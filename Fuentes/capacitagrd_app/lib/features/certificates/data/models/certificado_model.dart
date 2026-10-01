import 'package:equatable/equatable.dart';

class CertificadoModel extends Equatable {
  final int idEvento;
  final String nombreEvento;
  final String nombrePersona;
  final String numDocumento;
  final DateTime fechaEmision;
  final String? urlPdf;

  const CertificadoModel({
    required this.idEvento,
    required this.nombreEvento,
    required this.nombrePersona,
    required this.numDocumento,
    required this.fechaEmision,
    this.urlPdf,
  });

  factory CertificadoModel.fromJson(Map<String, dynamic> j) => CertificadoModel(
        idEvento: j['iD_EVENTO'] ?? 0,
        nombreEvento: j['nombrE_EVENTO'] ?? j['nombre'] ?? '',
        nombrePersona: j['nombrE_PERSONA'] ?? '',
        numDocumento: j['nuM_DOCUMENTO'] ?? '',
        fechaEmision: DateTime.tryParse(j['fechA_EMISION'] ?? '') ?? DateTime.now(),
        urlPdf: j['urL_PDF'],
      );

  @override
  List<Object?> get props => [idEvento, numDocumento];
}

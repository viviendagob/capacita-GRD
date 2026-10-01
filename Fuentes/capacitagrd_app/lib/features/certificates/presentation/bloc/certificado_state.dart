part of 'certificado_cubit.dart';

abstract class CertificadoState extends Equatable {
  const CertificadoState();
  @override
  List<Object?> get props => [];
}

class CertificadoInitial extends CertificadoState {}
class CertificadoLoading extends CertificadoState {}
class CertificadoDescargando extends CertificadoState {}

class CertificadosLoaded extends CertificadoState {
  final List<CertificadoModel> certificados;
  const CertificadosLoaded(this.certificados);
  @override
  List<Object?> get props => [certificados];
}

class CertificadoUrlObtenido extends CertificadoState {
  final String url;
  const CertificadoUrlObtenido(this.url);
  @override
  List<Object?> get props => [url];
}

class CertificadoError extends CertificadoState {
  final String message;
  const CertificadoError(this.message);
  @override
  List<Object?> get props => [message];
}

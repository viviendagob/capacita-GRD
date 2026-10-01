import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import '../../data/models/certificado_model.dart';
import '../../domain/repositories/certificado_repository.dart';

part 'certificado_state.dart';

class CertificadoCubit extends Cubit<CertificadoState> {
  final CertificadoRepository repository;

  CertificadoCubit({required this.repository}) : super(CertificadoInitial());

  Future<void> cargarMisCertificados(int idPersona) async {
    emit(CertificadoLoading());
    final result = await repository.getMisCertificados(idPersona);
    result.fold(
      (failure) => emit(CertificadoError(failure.message)),
      (list) => emit(CertificadosLoaded(list)),
    );
  }

  Future<void> descargarCertificado(int idEvento, int idPersona) async {
    emit(CertificadoDescargando());
    final result = await repository.getUrlCertificado(idEvento, idPersona);
    result.fold(
      (failure) => emit(CertificadoError(failure.message)),
      (url) => emit(CertificadoUrlObtenido(url)),
    );
  }
}

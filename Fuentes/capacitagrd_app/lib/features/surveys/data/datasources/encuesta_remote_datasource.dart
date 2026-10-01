import 'package:dio/dio.dart';
import '../../../../core/constants/app_constants.dart';
import '../models/encuesta_model.dart';

abstract class EncuestaRemoteDataSource {
  Future<EncuestaModel> getEncuesta(int idEncuesta);
  Future<void> responder(List<RespuestaParticipanteModel> respuestas);
  Future<bool> completada(int idEvento, int idPersona);
}

class EncuestaRemoteDataSourceImpl implements EncuestaRemoteDataSource {
  final Dio dio;
  const EncuestaRemoteDataSourceImpl({required this.dio});

  @override
  Future<EncuestaModel> getEncuesta(int idEncuesta) async {
    final response = await dio.get('${ApiRoutes.encuestas}/$idEncuesta');
    return EncuestaModel.fromJson(response.data);
  }

  @override
  Future<void> responder(List<RespuestaParticipanteModel> respuestas) async {
    await dio.post(
      ApiRoutes.eventoEncuestasResponder,
      data: respuestas.map((r) => r.toApiJson()).toList(),
    );
  }

  @override
  Future<bool> completada(int idEvento, int idPersona) async {
    final response = await dio.get(
      ApiRoutes.eventoEncuestasCompletada,
      queryParameters: {'idEvento': idEvento, 'idPersona': idPersona},
    );
    return response.data == true;
  }
}

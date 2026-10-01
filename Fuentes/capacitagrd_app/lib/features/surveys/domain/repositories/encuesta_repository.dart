import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../data/models/encuesta_model.dart';

abstract class EncuestaRepository {
  Future<Either<Failure, EncuestaModel>> getEncuesta(int idEncuesta);
  Future<Either<Failure, void>> guardarRespuestasLocal(List<RespuestaParticipanteModel> respuestas);
  Future<Either<Failure, List<RespuestaParticipanteModel>>> getRespuestasLocal(
      int idEncuesta, int idPersona, int idEvento);

  // Envía las respuestas al servidor. Si falla por conectividad, quedan guardadas
  // localmente (ya se guardan antes de llamar esto) para reintentar más tarde.
  Future<Either<Failure, void>> enviarRespuestas(List<RespuestaParticipanteModel> respuestas);
}

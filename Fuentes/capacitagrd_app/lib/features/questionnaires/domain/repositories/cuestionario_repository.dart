import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../data/models/cuestionario_model.dart';

abstract class CuestionarioRepository {
  Future<Either<Failure, CuestionarioModel>> getCuestionario(int idCuestionario);
  Future<Either<Failure, void>> guardarRespuestasLocal(List<RespuestaCuestionarioModel> respuestas);
  Future<Either<Failure, List<RespuestaCuestionarioModel>>> getRespuestasLocal(
      int idCuestionario, int idPersona, int idEvento);
}

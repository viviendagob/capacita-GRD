import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../data/models/asistencia_model.dart';

abstract class AsistenciaRepository {
  Future<Either<Failure, List<AsistenciaModel>>> getAsistencias(int idEvento, int idPersona);
  Future<Either<Failure, AsistenciaModel>> registrarAsistencia(RegistrarAsistenciaRequest request);
}

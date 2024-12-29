[assembly: DoNotParallelize] // Sequential unit tests for everyone
////[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]  // Causes collisions across classes for in-memory tests
////[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)] // Causes collisions all over

/* Copyright Xeno Innovations, Inc. 2019-2025
 * Date:    2019-9-15
 * Author:  Damian Suess
 * File:    MsTestSettings.cs
 * Description:
 *  MSTest settings
 */

// All unit tests 
[assembly: DoNotParallelize] // Sequential unit tests for everyone
////[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]  // Causes collisions across classes for in-memory tests
////[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)] // Causes collisions all over

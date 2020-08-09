## How to Contribute
1. Fork on GitHub
2. Create a branch
3. Code (_and add tests)
4. Create a Pull Request (_PR_) on GitHub
   1. Target the ``develop`` branch and we'll get it merged up to ``master``
   2. Target the ``master`` branch for hotfixes
5. Get the PR merged
6. Welcome to our contributors' list!

## How To Build
```
dotnet build source\SqlMigrator.sln
```

## How to Test
```
dotnet test source\SqlMigrator.Tests\SqlMigrator.SystemTests.csproj
```

## Coding Style
This project uses [EditorConfig](https://raw.githubusercontent.com/fluentmigrator/fluentmigrator/master/.editorconfig) to keep the coding style common. If your IDE doesn't support ``.editorconfig`` files then please tidy up before contributing.

* Indent type: ``spaces``
* Indent size: ``2``
* Prefer braces
* Sort ``using`` directives
* ``System`` directives come first
* Add a license to header region of every ``.cs`` file
* Empty line after a closing brace
* Line break before an open brace

```cs
// Copyright (c) 2020, LiteMigrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Description:
//  << quick code description >>
```

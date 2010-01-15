task :default => :build

desc "Build the C# code with MSBuild"
task :build do
  msbuild = 'c:\windows\microsoft.net\framework\v3.5\msbuild.exe'
  solution = File.expand_path(File.dirname(__FILE__) + '/Cuke4Nuke/Cuke4Nuke.sln')
  sh %{#{msbuild} "#{solution}" /p:configuration=Release}
end

desc "Run the unit tests with NUnit"
task :test do
  nunit = 'C:\Program Files\NUnit 2.5.2\bin\net-2.0\nunit-console.exe'
  tests = File.expand_path(File.dirname(__FILE__) + '/Cuke4Nuke/Specifications/bin/Release/Cuke4Nuke.Specifications.dll')
  sh %{"#{nunit}" "#{tests}"}
end

desc "Run the install"
task :install => :build do
  # I'm sure there's a better way to do this
  cd "gem"
  sh %{rake install}
  cd ".."
end

desc "Run the features with cucumber"
task :cucumber => :install do
  sh "cucumber"
end

task :log => 'log:less'

namespace :log do
  desc "Clear log"
  task :clear do
    `rm -rf #{log}`
  end

  desc "View log in less"
  task :less do
    exec %{less #{log}}
  end
end

def log
  file = File.expand_path(File.dirname(__FILE__) + '/Cuke4Nuke/Server/bin/Release/Cuke4NukeLog.txt')
  %{"#{file}"}
end


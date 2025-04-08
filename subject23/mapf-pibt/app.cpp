int main(int argc, char* argv[])
{
    return 0;
}

// #include <default_params.hpp>
// #include <iostream>
// #include <ir.hpp>
// #include <pibt.hpp>
// #include <problem.hpp>
//
// void printHelp();
// std::unique_ptr<Solver> getSolver(const std::string solver_name, Problem* P,
//                                   bool verbose, int argc, char* argv[]);
//
// int main(int argc, char* argv[])
// {
//     std::string instance_file = "";
//     std::string output_file = DEFAULT_OUTPUT_FILE;
//     std::string solver_name;
//     bool verbose = false;
//     char* argv_copy[argc + 1];
//     for (int i = 0; i < argc; ++i)
//         argv_copy[i] = argv[i];
//
//     struct option longopts[] = {
//         { "instance", required_argument, 0, 'i' },
//         { "output", required_argument, 0, 'o' },
//         { "solver", required_argument, 0, 's' },
//         { "verbose", no_argument, 0, 'v' },
//         { "help", no_argument, 0, 'h' },
//         { "time-limit", required_argument, 0, 'T' },
//         { "make-scen", no_argument, 0, 'P' },
//         { 0, 0, 0, 0 },
//     };
//     bool make_scen = false;
//     int max_comp_time = -1;
//
//     // command line args
//     int opt, longindex;
//     opterr = 0; // ignore getopt error
//     while ((opt = getopt_long(argc, argv, "i:o:s:vhPT:", longopts,
//     &longindex))
//            != -1)
//     {
//         switch (opt)
//         {
//         case 'i':
//             instance_file = std::string(optarg);
//             break;
//         case 'o':
//             output_file = std::string(optarg);
//             break;
//         case 's':
//             solver_name = std::string(optarg);
//             break;
//         case 'v':
//             verbose = true;
//             break;
//         case 'h':
//             printHelp();
//             return 0;
//         case 'P':
//             make_scen = true;
//             break;
//         case 'T':
//             max_comp_time = std::atoi(optarg);
//             break;
//         default:
//             break;
//         }
//     }
//
//     if (instance_file.length() == 0)
//     {
//         std::cout << "specify instance file using -i [INSTANCE-FILE], e.g.,"
//                   << std::endl;
//         std::cout << "> ./app -i ../instance/sample.txt" << std::endl;
//         return 0;
//     }
//
//     // set problem
//     Problem P = Problem(instance_file);
//
//     // set max computation time (otherwise, use param in instance_file)
//     if (max_comp_time != -1)
//         P.setMaxCompTime(max_comp_time);
//
//     // create scenario
//     if (make_scen)
//     {
//         P.makeScenFile(output_file);
//         return 0;
//     }
//
//     // solve
//     auto solver = getSolver(solver_name, &P, verbose, argc, argv_copy);
//     solver->solve();
//     if (solver->succeed())
//         std::cout << "succeed";
//     if (solver->succeed() && !solver->getSolution().validate(&P))
//     {
//         std::cout << "error@app: invalid results" << std::endl;
//         return 0;
//     }
//     solver->printResult();
//
//     // output result
//     solver->makeLog(output_file);
//     if (verbose)
//     {
//         std::cout << "save result as " << output_file << std::endl;
//     }
//
//     return 0;
// }
//
// std::unique_ptr<Solver> getSolver(const std::string solver_name, Problem* P,
//                                   bool verbose, int argc, char* argv[])
// {
//     std::unique_ptr<Solver> solver;
//     if (solver_name == "PIBT")
//     {
//         solver = std::make_unique<PIBT>(P);
//     }
//     else if (solver_name == "IR")
//     {
//         solver = std::make_unique<IR>(P);
//     }
//     else if (solver_name == "IR_SINGLE_PATHS")
//     {
//         solver = std::make_unique<IR_SINGLE_PATHS>(P);
//     }
//     else if (solver_name == "IR_FIX_AT_GOALS")
//     {
//         solver = std::make_unique<IR_FIX_AT_GOALS>(P);
//     }
//     else if (solver_name == "IR_FOCUS_GOALS")
//     {
//         solver = std::make_unique<IR_FOCUS_GOALS>(P);
//     }
//     else if (solver_name == "IR_MDD")
//     {
//         solver = std::make_unique<IR_MDD>(P);
//     }
//     else if (solver_name == "IR_BOTTLENECK")
//     {
//         solver = std::make_unique<IR_BOTTLENECK>(P);
//     }
//     else if (solver_name == "IR_HYBRID")
//     {
//         solver = std::make_unique<IR_HYBRID>(P);
//     }
//     else
//     {
//         std::cout << "warn@app: "
//                   << "unknown solver name, " + solver_name
//                 + ", continue by PIBT"
//                   << std::endl;
//         solver = std::make_unique<PIBT>(P);
//     }
//     solver->setParams(argc, argv);
//     solver->setVerbose(verbose);
//     return solver;
// }
//
// void printHelp()
// {
//     std::cout << "\nUsage: ./app [OPTIONS] [SOLVER-OPTIONS]\n"
//               << "\n**instance file is necessary to run MAPF simulator**\n\n"
//               << "  -i --instance [FILE_PATH]     instance file path\n"
//               << "  -o --output [FILE_PATH]       ouptut file path\n"
//               << "  -v --verbose                  print additional info\n"
//               << "  -h --help                     help\n"
//               << "  -s --solver [SOLVER_NAME]     solver, choose from the
//         below\n "
//               << "  -T --time-limit [INT]         max computation time
//               (ms)\n"
//               << "  -P --make-scen                make scenario file using "
//                  "random starts/goals"
//               << "\n\nSolver Options:" << std::endl;
//     // each solver
//     PIBT::printHelp();
//     IR::printHelp();
//     IR_SINGLE_PATHS::printHelp();
//     IR_FIX_AT_GOALS::printHelp();
//     IR_FOCUS_GOALS::printHelp();
//     IR_MDD::printHelp();
//     IR_BOTTLENECK::printHelp();
//     IR_HYBRID::printHelp();
// }

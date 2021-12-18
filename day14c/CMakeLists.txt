cmake_minimum_required(VERSION 3.21)
project(day14c C)

set(CMAKE_C_STANDARD 23)

configure_file(sample sample COPYONLY)
configure_file(input input COPYONLY)
add_executable(${PROJECT_NAME} main.c)

CC=clang
CFLAGS=-I. -Wall -Wextra -O3 -ffast-math
PROGRAM=$(shell basename "$(PWD)")
OBJ = $(PROGRAM).o

%.o: %.c 
	$(CC) -c -o $@ $< $(CFLAGS)

$(PROGRAM): $(OBJ)
	$(CC) -o $@ $^ $(CFLAGS)

run: $(PROGRAM)
	./$(PROGRAM)

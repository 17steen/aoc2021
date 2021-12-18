#include <stdio.h>
#include <float.h>
#include <stdbool.h>
#include <ctype.h>
#include <stdlib.h>
#include <stdint.h>
#include <assert.h>
#include <string.h>

//  26 * 26
#define POSSIBILITIES 676

static int8_t parse(const char *file_name, uint64_t template[POSSIBILITIES], int8_t insertions[POSSIBILITIES]) {
    FILE *filePointer;
    int bufferLength = 255;
    char buffer[bufferLength];

    filePointer = fopen(file_name, "r");
    if (!filePointer) {
        puts("file not found");
        abort();
    }

    fgets(buffer, bufferLength, filePointer);
    int8_t first_letter = (char)(buffer[0] - 'A');
    for (int i = 1; buffer[i] && buffer[i] != '\n'; ++i) {
        char a = buffer[i - 1];
        char b = buffer[i];

#ifdef DEBUG
        printf("pair: %c%c\n", a, b);
#endif

        size_t index = (a - 'A') * 26 + (b - 'A');
        template[index] += 1;
    }

    fgets(buffer, bufferLength, filePointer);
    while (fgets(buffer, bufferLength, filePointer)) {

        char c_a = buffer[0];
        char c_b = buffer[1];
        char c_c = buffer[6];

#ifdef DEBUG
        printf("insertions: %c%c -> %c\n", c_a, c_b, c_c);
        assert(isalpha(c_a) && isalpha(c_b) && isalpha(c_c));
#endif

        int8_t a = (char) (c_a - 'A');
        int8_t b = (char) (c_b - 'A');
        int8_t c = (char) (c_c - 'A');


        size_t index = a * 26 + b;
        insertions[index] = (int8_t) c ;
    }

    fclose(filePointer);
    return first_letter;
}

inline static void
step(const uint64_t template[POSSIBILITIES], uint64_t output[POSSIBILITIES], const int8_t insertions[POSSIBILITIES]) {
    memset(output, 0, POSSIBILITIES * sizeof(*output));
    for (int i = 0; i < POSSIBILITIES; ++i) {
        if (template[i]) {
            int left = i / 26;
            int right = i % 26;

            int8_t middle = insertions[i];

            size_t index1 = left * 26 + middle;
            size_t index2 = middle * 26 + right;

            output[index1] += template[i];
            output[index2] += template[i];
        }
    }
}


double solve(int step_count, char const *file_name) {
    // maps a pair to its count
    uint64_t buffers[2][POSSIBILITIES] = {0};
    bool current_buffer = 0;
    // maps a pair to its insertion result
    int8_t insertions[POSSIBILITIES] = {0};
    //maybe keep track of the current templates so as not to iterate over the whole thing
    int8_t first_letter = parse(file_name, buffers[current_buffer], insertions);

    for (int i = 0; i < step_count; ++i) {
        step(buffers[current_buffer], buffers[!current_buffer], insertions);
        current_buffer = !current_buffer;
    }

    // count letters
    // using doubles because integers overflow
    double letters[26] = { 0 };
    letters[first_letter] = 1; // let’s not forget to use the first letter

    uint64_t *template = buffers[current_buffer];
    for (int i = 0; i < POSSIBILITIES; ++i) {
        int right = i % 26;
        letters[right] += (double) template[i]; // where it gets big
    }

    double least = DBL_MAX;
    double most = DBL_MIN;
    for (int i = 0; i < 26; ++i) {
        if (letters[i]) {
            if (letters[i] < least)
                least = letters[i];
            if (letters[i] > most)
                most = letters[i];
        }
    }
    double solved = most - least;

#ifdef DEBUG
    //printf("amount of times B occurs: %lu\n", letters['B' - 'A']);
    printf("most %lu least %lu\n", most, least);
    assert(most > least);
    printf("%lu\n", solved);
#endif

    return solved;
}

int main() {
    assert(solve(10, "sample") == 1588.);
    assert(solve(40, "sample") == 2188189693529.);
    printf("%lf\n", solve(10, "input"));
    printf("%lf\n", solve(40, "input"));
    printf("%lf\n", solve(200, "input"));
    return 0;
}

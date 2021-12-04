#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <stdbool.h>
#include <time.h>

typedef struct
{
    char *str;
    size_t len;

} String;

typedef struct
{
    String *lines;
    size_t len;
} Lines;

void free_lines(Lines lines)
{
    for (size_t i = 0; i < lines.len; ++i)
        free(lines.lines[i].str);
    free(lines.lines);
}

Lines get_input(const char *filename)
{
    FILE *file = fopen(filename, "r");
    char *lineptr = NULL;
    size_t size = 0;
    String *lines = NULL;
    size_t count = 0;
    size_t read_len = 0;

    long int getline(char **restrict lineptr, size_t *restrict n,
                       FILE *restrict stream);

    while ((read_len = getline(&lineptr, &size, file)) != -1)
    {
        ++count;
        lines = realloc(lines, sizeof(String) * count);
        lines[count - 1] = (String){.len = read_len, .str = lineptr};
        lineptr = NULL;
    }

    return (Lines){.lines = lines, .len = count};
}

size_t part1(Lines input)
{
    size_t gamma = 0;
    size_t epsilon = 0;

    size_t len = input.len;

    for (size_t j = 0; j < input.lines[0].len - 1; ++j)
    {
        size_t count = 0;
        for (size_t i = 0; i < len; ++i)
        {
            char ch = input.lines[i].str[j];
            count += ch == '1';
        }
        bool mostCommon = count >= (len / 2);

        gamma = (gamma << 1) | mostCommon;
        epsilon = (epsilon << 1) | !mostCommon;
    }

    return gamma * epsilon;
}

size_t part2(Lines input)
{
    size_t gamma = 0;
    size_t epsilon = 0;

    size_t len = input.len;

    bool *valids = calloc(sizeof(bool), len);
    size_t valid_count = len;
    char chosen = '1';

    char * valid_str = NULL;

    for (size_t j = 0; j < (input.lines[0].len - 1); ++j)
    {

        printf("%lu %lu %d\n", j, input.lines[0].len - 1,(int) (j < (input.lines[0].len - 1)));

        size_t count = 0;
        for (size_t i = 0; i < len; ++i)
        {
            if (!valids[i])
                continue;

            char ch = input.lines[i].str[j];
            count += ch == '1';
        }
        bool mostCommon = count >= (len / 2);

        for (size_t i = 0; i < len; ++i)
        {
            if (!valids[i])
                continue;

            char ch = input.lines[i].str[j];
            if (ch != chosen)
            {
                valid_count -= 1;
            }
        }

        if (valid_count == 1)
        {
            for (size_t i = 0; i < len; ++i)
                if (!valids[i])
                    continue;
                else
                    valid_str = input.lines[i].str;
            break;
        }
    }

    puts(valid_str);

    free(valids);

    return 0;
}

int main()
{
    Lines input = get_input("sample");
    clock_t start_time = clock();
    size_t result = part1(input);
    size_t result2 = part2(input);
    double elapsed_time = (double)(clock() - start_time) / CLOCKS_PER_SEC * 1000;
    printf("%d in %lfµs\n", result);
    printf("%d in %lfµs\n", result2);
    free_lines(input);
}
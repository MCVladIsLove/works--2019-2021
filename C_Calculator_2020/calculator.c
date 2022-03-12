#include <stdio.h>
#include <ctype.h>
#include <math.h>
#include <stdlib.h>
#include <locale.h>
#include <string.h>                                    //Библиотеки
#define M_PI       3.14159265358979323846              //Константы
#define LIM 200






void reverse(s);
char* numtos(a);
int replace(s, number, i, max);
char* vichislit(ptr_c);
double getnum(s, side, i, lim);                   //Функции



int main()
{
	char c = 1;
	system("chcp 1251");
	system("cls");
	printf("(Нажмите Ctrl+Z, чтобы выйти)\n");
	printf("\t\t\tДоступные команды:\n");
	printf("'+' - плюс\t\t\t");
	printf("'-' - минус\t\t");
	printf("'P' - число Пи\n");
	printf("'*' - умножение\t\t\t");
	printf("'/' - деление\t\t'!' - факториал\n");
	printf("'%c' - деление с остатком\t", 37);
	printf("'^' - возведение в степень\n\n");
	printf("sin, cos, tg, ctg, arctg, arcctg, arcsin, arccos - тригонометрические функции\n\n");   //Инструкция
	printf("root - корень произвольной степени\t(root\"число\"  '|'  \"степень корня\")\n");
	printf("ln - натуральный логарифм\tlg - десятичный логарифм\n");
	printf("log - произвольный логарифм\t(log\"основание\"  '|'   \"число\")\n\n\n");

	while (c != EOF)
	{
		printf("Введите выражение:\n");
		printf("\nОтвет:%s\n\n\n", vichislit(&c));
	}
	return 0;
}




char* vichislit(char* ptr_c)                           //Функция вычисления
{
	int i = 0, tmp;                            //Счётчики
	double a = 0, b = 0;                     //Переменные для мат. операций
	double num = 0;                          //Результат вычислений
	char* mass = (char*)calloc(LIM, 1);                  //Динамический массив
	while ((*ptr_c = getchar()) != EOF && *ptr_c != '\n' && *ptr_c != ')' && i < LIM - 1)                              //Заполнение массива
	{
		if (isspace(*ptr_c))                                                        //Пропускаем пробелы, табуляции и тд
			continue;
		if (*ptr_c == '(')                                         //Если есть '('
		{
			if (isdigit(mass[i - 1]) || mass[i - 1] == 'P')
				mass[i++] = '*';
			strcpy(&mass[i], vichislit(ptr_c));                                    //Рекурсия
			i = strlen(mass);
			continue;
		}
		if (isalpha(*ptr_c) && *ptr_c != 'P' && (isdigit(mass[i - 1]) || mass[i - 1] == 'P'))   //Умножение
			mass[i++] = '*';
		if (i > 0 && mass[i - 1] == '-' && *ptr_c != 'P' && (isalpha(*ptr_c) || *ptr_c == '!'))  //Минус
		{
			mass[i++] = '1';
			mass[i++] = '*';
		}
		mass[i] = *ptr_c;
		++i;
	}
	mass[i] = '\0';
	a = getnum(mass, 0, i - 1);
	for (tmp = 0; tmp <= i; ++tmp)                                      //Корни,синусы, логарифмы
	{
		if (mass[tmp] == 'r')
			if (!strncmp(&mass[tmp], "root", 4))
			{
				while (mass[tmp] != '|')
					++tmp;
				a = getnum(mass, 0, tmp - 1);
				b = getnum(mass, 1, tmp + 1);
				num = pow(a, 1 / b);
				i = replace(mass, numtos(num), tmp, i);
				tmp = 0;
			}
			else printf("ОШИБКА: НЕИЗВЕСТНАЯ КОМАНДА");


		if (tmp > 0 && mass[tmp] == 's' && mass[tmp - 1] != 'c')
			if (!strncmp(&mass[tmp], "sin", 3))
			{
				b = getnum(mass, 1, tmp + 3);
				num = sin(b);
				i = replace(mass, numtos(num), tmp + 2, i);
				tmp = 0;
			}
			else printf("ОШИБКА: НЕИЗВЕСТНАЯ КОМАНДА");
		else if(tmp == 0 && mass[tmp] == 's')
			if (!strncmp(&mass[tmp], "sin", 3))
			{
				b = getnum(mass, 1, tmp + 3);
				num = sin(b);
				i = replace(mass, numtos(num), tmp + 2, i);
				tmp = 0;
			}
			else printf("ОШИБКА: НЕИЗВЕСТНАЯ КОМАНДА");


		if (tmp > 0 && mass[tmp] == 'c' && mass[tmp - 1] != 'c')
			if (!strncmp(&mass[tmp], "cos", 3))
			{
				b = getnum(mass, 1, tmp + 3);
				num = cos(b);
				i = replace(mass, numtos(num), tmp + 2, i);
				tmp = 0;
			}
			else if (!strncmp(&mass[tmp], "ctg", 3))
			{
				b = getnum(mass, 1, tmp + 3);
				num = tan(M_PI / 2 - b);
				if (b == M_PI)
					printf("ОШИБКА: НЕДОПУСТИМОЕ ЗНАЧЕНИЕ КОТАНГЕНСА");
				i = replace(mass, numtos(num), tmp + 2, i);
				tmp = 0;
			}
			else printf("ОШИБКА: НЕИЗВЕСТНАЯ КОМАНДА");
		else if (tmp == 0 && mass[tmp] == 'c')
			if (!strncmp(&mass[tmp], "cos", 3))
			{
				b = getnum(mass, 1, tmp + 3);
				num = cos(b);
				i = replace(mass, numtos(num), tmp + 2, i);
				tmp = 0;
			}
			else if (!strncmp(&mass[tmp], "ctg", 3))
			{
				b = getnum(mass, 1, tmp + 3);
				num = tan(M_PI / 2 - b);
				if (b == M_PI)
					printf("ОШИБКА: НЕДОПУСТИМОЕ ЗНАЧЕНИЕ КОТАНГЕНСА");
				i = replace(mass, numtos(num), tmp + 2, i);
				tmp = 0;
			}
			else printf("ОШИБКА: НЕИЗВЕСТНАЯ КОМАНДА");


		if (tmp > 0 && mass[tmp] == 't' && mass[tmp - 1] != 'c')
			if (!strncmp(&mass[tmp], "tg", 2))
			{
				b = getnum(mass, 1, tmp + 2);
				num = tan(b);
				if (b == M_PI / 2)
					printf("ОШИБКА: НЕДОПУСТИМОЕ ЗНАЧЕНИЕ ТАНГЕНСА");
				i = replace(mass, numtos(num), tmp + 1, i);
				tmp = 0;
			}
			else printf("ОШИБКА: НЕИЗВЕСТНАЯ КОМАНДА");
		else if (tmp == 0 && mass[tmp] == 't')
			if (!strncmp(&mass[tmp], "tg", 2))
			{
				b = getnum(mass, 1, tmp + 2);
				num = tan(b);
				if (b == M_PI / 2)
					printf("ОШИБКА: НЕДОПУСТИМОЕ ЗНАЧЕНИЕ ТАНГЕНСА");
				i = replace(mass, numtos(num), tmp + 1, i);
				tmp = 0;
			}
			else printf("ОШИБКА: НЕИЗВЕСТНАЯ КОМАНДА");


		if (mass[tmp] == 'a')
			if (!strncmp(&mass[tmp], "arcsin", 6))
			{
				b = getnum(mass, 1, tmp + 6);
				num = asin(b);
				i = replace(mass, numtos(num), tmp + 5, i);
				tmp = 0;
			}
			else if (!strncmp(&mass[tmp], "arccos", 6))
			{
				b = getnum(mass, 1, tmp + 6);
				num = acos(b);
				i = replace(mass, numtos(num), tmp + 5, i);
				tmp = 0;
			}
			else if (!strncmp(&mass[tmp], "arcctg", 6))
			{
				b = getnum(mass, 1, tmp + 6);
				num = M_PI / 2 - atan(b);
				i = replace(mass, numtos(num), tmp + 5, i);
				tmp = 0;
			}
			else if (!strncmp(&mass[tmp], "arctg", 5))
			{
				b = getnum(mass, 1, tmp + 5);
				num = atan(b);
				i = replace(mass, numtos(num), tmp + 4, i);
				tmp = 0;
			}
			else printf("ОШИБКА: НЕИЗВЕСТНАЯ КОМАНДА");


		if (mass[tmp] == 'l')
			if (!strncmp(&mass[tmp], "log", 3))
			{
				while (mass[tmp] != '|')
					++tmp;
				a = getnum(mass, 0, tmp - 1);
				b = getnum(mass, 1, tmp + 1);
				if (a <= 0 || b <= 0)
					printf("ОШИБКА: НОЛЬ ИЛИ ОТРИЦАТЕЛЬНОСТЬ В ЛОГАРИФМЕ");
				num = log2(b) / log2(a);
				i = replace(mass, numtos(num), tmp, i);
				tmp = 0;
			}
			else if (!strncmp(&mass[tmp], "ln", 2))
			{
				b = getnum(mass, 1, tmp + 2);
				num = log(b);
				if (b == 0)
					printf("ОШИБКА: ЛОГАРИФМ ОТ НУЛЯ");
				else if (b < 0)
					printf("ОШИБКА: ЛОГАРИФМ ОТ ОТРИЦАТЕЛЬНОГО ЧИСЛА");
				i = replace(mass, numtos(num), tmp + 1, i);
				tmp = 0;
			}
			else if (!strncmp(&mass[tmp], "lg", 2))
			{
				b = getnum(mass, 1, tmp + 2);
				num = log10(b);
				if (b == 0)
					printf("ОШИБКА: ЛОГАРИФМ ОТ НУЛЯ");
				else if (b < 0)
					printf("ОШИБКА: ЛОГАРИФМ ОТ ОТРИЦАТЕЛЬНОГО ЧИСЛА");
				i = replace(mass, numtos(num), tmp + 1, i);
				tmp = 0;
			}
			else printf("ОШИБКА: НЕИЗВЕСТНАЯ КОМАНДА");
	}

	for (tmp = 0; tmp <= i; ++tmp)
		if (mass[tmp] == '!')
		{
			num = 1;
			b = getnum(mass, 1, tmp + 1);
			if (b < 0)
				printf("ОШИБКА: ФАКТОРИАЛ ОТРИЦАТЕЛЬНОГО ЧИСЛА");
			while (b > 0)
			{
				num *= b;
				b--;
			}
			i = replace(mass, numtos(num), tmp, i);
			tmp = 0;
		}
	for (tmp = i; tmp >= 0; --tmp)
	{
		if (mass[tmp] == '^')
		{
			a = getnum(mass, 0, tmp - 1);
			b = getnum(mass, 1, tmp + 1);
			num = pow(a, b);
			i = replace(mass, numtos(num), tmp, i);
			tmp = i;
		}
	}
	for (tmp = 0; tmp <= i; ++tmp)
	{
		if (mass[tmp] == '%')
		{
			a = getnum(mass, 0, tmp - 1);
			b = getnum(mass, 1, tmp + 1);
			num = (long long)a % (long long)b;
			if (a > (long long)a || b > (long long)b)
				printf("\nПРЕДУПРЕЖДЕНИЕ: ВОЗМОЖНЫ НЕТОЧНОСТИ В ОТВЕТЕ\nИЗ-ЗА ДЕЛЕНИЯ С ОСТАТКОМ НЕЦЕЛЫХ ЧИСЕЛ");
			i = replace(mass, numtos(num), tmp, i);
			tmp = 0;
		}
	}
	for (tmp = 0; tmp <= i; ++tmp)
	{
		if (mass[tmp] == '/')
		{
			a = getnum(mass, 0, tmp - 1);
			b = getnum(mass, 1, tmp + 1);
			num = a / b;
			if (b == 0)
				printf("ОШИБКА: ДЕЛЕНИЕ НА НОЛЬ");
			i = replace(mass, numtos(num), tmp, i);
			tmp = 0;
		}
	}
	for (tmp = 0; tmp <= i; ++tmp)
	{
		if (mass[tmp] == '*')
		{
			a = getnum(mass, 0, tmp - 1);
			b = getnum(mass, 1, tmp + 1);
			num = a * b;
			i = replace(mass, numtos(num), tmp, i);
			tmp = 0;
		}
	}
	for (tmp = 0; tmp <= i; ++tmp)
	{
		if (mass[tmp] == '+')
		{
			a = getnum(mass, 0, tmp - 1);
			b = getnum(mass, 1, tmp + 1);
			num = a + b;
			i = replace(mass, numtos(num), tmp, i);
			tmp = 0;
		}
		else if (mass[tmp] == '-' && tmp != 0)
		{
			a = getnum(mass, 0, tmp - 1);
			b = getnum(mass, 1, tmp + 1);
			num = a - b;
			i = replace(mass, numtos(num), tmp, i);
			tmp = 0;
		}
	}
	return mass;
}






double getnum(char* s, int side, int i)        //Получаю числа a и b
{
	int j = 1, minus = 0, p = 0, zero = 1;
	double n = 0;
	if (side == 1)
	{
		if (s[i] == '-')
		{
			minus = 1;
			++i;
		}
		if (s[i] == 'P')
		{
			p = 1;
			++i;
		}
		for (i; isdigit(s[i]); ++i)
		{
			n = n * 10 + s[i] - '0';
			zero = 0;
		}
		if (s[i] == '.' || s[i] == ',')
		{
			for (i = i + 1; isdigit(s[i]); ++i)
			{
				n += ((double)s[i] - '0') / pow(10, j);
				j += 1;
			}
		}
		if (s[i] == 'P' && !zero)
			n *= M_PI;
		else if (s[i] == 'P' && n == 0)
			n = M_PI;
	}
	else
	{
		j = 0;
		if (s[i] == 'P')
		{
			p = 1;
			--i;
		}
		for (i; i >= 0 && isdigit(s[i]); --i)
		{
			n += ((double)s[i] - '0') * pow(10, j);
			j += 1;
			zero = 0;
		}
		if (s[i] == '.' || s[i] == ',')
		{
			n /= pow(10, j);
			j = 0;
			for (i = i - 1; i >= 0 && isdigit(s[i]); --i)
			{
				n += ((double)s[i] - '0') * pow(10, j);
				j += 1;
			}
		}
		if (s[i] == 'P')
		{
			p = 1;
			i--;
		}
		if (i > 0 && s[i] == '-')
		{
			if (!isdigit(s[i - 1]))
				minus = 1;
		}
		else if (i == 0 && s[i] == '-')
			minus = 1;
	}	
	if (p && !zero)
		n *= M_PI;
	else if (p && n == 0)
		n = M_PI;
	if (minus == 1)
		n = -n;
	return n;
}




int replace(char* s, char* number, int i, int max)                                 //Заменяю использованные числа для массива
{
	int j = strlen(number);
	int tmp = i;
	int leftb, rightb;
	--i;
	while (i >= 0 && (isdigit(s[i]) || s[i] == ',' || s[i] == '.' || isalpha(s[i])))
		--i;
	leftb = i + 1;
	if (i > 0 && s[i] == '-' && !isdigit(s[i - 1]) && s[i - 1] != 'P')
		leftb = i;
	else if (i == 0 && s[i] == '-')
		leftb = i;
	i = tmp + 1;
	if (i < LIM - 1 && s[i] == '-' && i <= max)
		++i;
	while (i < LIM - 1 && i <= max && (isdigit(s[i]) || s[i] == ',' || s[i] == '.' || s[i] == 'P'))
		++i;
	rightb = i - 1;
	if (rightb - leftb > j)
		while (rightb - leftb != j)
		{
			for (tmp = rightb; tmp < max; ++tmp)
				s[tmp] = s[tmp + 1];
			s[tmp] = 0;
			--max;
			--rightb;
		}
	else if (rightb - leftb < j)
		while (rightb - leftb != j)
		{
			for (tmp = max + 1; tmp > rightb + 1; --tmp)                      
				s[tmp] = s[tmp - 1];
			++max;
			++rightb;
		}
	strcpy(&s[leftb], number);
	if (rightb < max)
	{
		for (rightb; rightb < max; ++rightb)
			s[rightb] = s[rightb + 1];
		--max;
	}
	return max;
}





char* numtos(double a)                                                     //Перевожу число в массив
{
	int j = 1, i = 0, min = 0, k;
	char* chislo = (char*)calloc(25, sizeof(char));
	if (a < 0)
	{
		a = -a;
		min = 1;
	}
	a *= pow(10, 10);
	for (k = 10; k >= 1 && (long long)a % 10 == 0; --k)
		a /= 10;
	do
	{
		chislo[i++] = (long long)a % 10 + '0';
	} while ((a = (long long)a / 10) != 0);
	if (k != 0)
	{
		if (i >= k)
		{
			if (min == 1)
				chislo[i++] = '-';
			chislo[i++] = '\0';
			while (i > k)
				chislo[i--] = chislo[i - 1];
			chislo[k] = '.';
		}
		else 
		{
			while (i != k)
				chislo[i++] = '0';
			chislo[i++] = '.';
			if (min == 1)
				chislo[i++] = '-';
			chislo[i] = '\0';
		}
	}
	else 
	{
		if (min == 1)
			chislo[i++] = '-';
		chislo[i] = '\0';
	}
	reverse(chislo);
	if (chislo[0] == '.' || (chislo[0] == '-' && chislo[1] == '.'))
	{
		i = strlen(chislo);
		while (chislo[i + 1] != '.')
		{
			chislo[i + 1] = chislo[i];
			--i;
		}
		chislo[i + 1] = '0';
	}
	return chislo;
}


void reverse(char* s)                                            //Переворачиваю число в массиве
{
	char tmp;
	int i = strlen(s) - 1;
	int j = 0;
	while (j < i)
	{
		tmp = s[j];
		s[j] = s[i];
		s[i] = tmp;
		++j;
		--i;
	}
}

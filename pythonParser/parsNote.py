import requests
from bs4 import BeautifulSoup
import xlwt

DNS = 'https://www.dns-shop.ru/'
URL = 'https://www.dns-shop.ru/catalog/17a892f816404e77/noutbuki/'
USER_AGENT = { 'user-agent': 'Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36'}


def makeTable(): # создает таблицу
    wb = xlwt.Workbook()
    ws = wb.add_sheet('Ноутбуки')
    ws.write(0, 0, 'Название')
    ws.write(0, 1, 'Цена')
    ws.write(0, 3, 'Разрешение экрана')
    ws.write(0, 4, 'Процессор')
    ws.write(0, 5, 'Ядер процессора')
    ws.write(0, 6, 'Потоков процессора')
    ws.write(0, 7, 'Частота процессора')
    ws.write(0, 8, 'Автоматическое увеличение частоты')
    ws.write(0, 9, 'Кэш L2')
    ws.write(0, 10, 'Кэш L3')
    ws.write(0, 11, 'Технологический процесс')
    ws.write(0, 12, 'Тип операвтивной памяти')
    ws.write(0, 13, 'Частота оперативной памяти')
    ws.write(0, 14, 'Размер оперативной памяти')
    ws.write(0, 15, 'Слотов под оперативную память')
    ws.write(0, 16, 'Максимальный объем памяти')
    ws.write(0, 17, 'Вид видеокарты')
    ws.write(0, 18, 'Видеокарта')
    ws.write(0, 19, 'Тип видеопамяти')
    ws.write(0, 20, 'Объем видеопамяти')
    ws.write(0, 21, 'Накопитель данных')
    ws.write(0, 22, 'Объем накопителя')
    ws.write(0, 23, 'Веб-камера')
    ws.write(0, 24, 'Микрофон')
    ws.write(0, 25, 'wi-fi')
    ws.write(0, 26, 'Приблизительное время автономной работы')
    ws.write(0, 27, 'Цифрофой блок клавиатуры')
    ws.write(0, 28, 'Баллы')
    ws.write(0, 29, 'Ссылка')
    return wb


def get_HTML(link, params=None): # получает HTML страницы
    r = requests.get(link, headers=USER_AGENT, params=params)
    return r.text


def getItems(price): # копирует ссылки на ноутбуки с каждой страницы и добавляет в список listOfItems
    soup = pars(get_HTML(URL, price))
    listOfItems = list('S' * 18 * 8 + 'S')
    i = 0
    numPages = numberOfPages(soup)
    for page in range(1, numPages + 1):
        for item in soup.find_all(class_='product-info__title-link'):
            listOfItems[i] = item.find(class_='ui-link').get('href') + 'characteristics/'
            i += 1
        soup = pars(get_HTML(URL, params='p='+str(page+1) + '&' + price))
    return listOfItems



def getPrice(parsedHtml):
    a = parsedHtml.find_all('script', type='text/javascript')[1]
    a = str(a)
    i = a.find('price') + 7
    cost = ''
    while a[i] != ',':
        cost += a[i]
        i += 1
    return cost


def numberOfPages(parsedHTML): # возвращает количество страниц
    pages = len(parsedHTML.find_all(class_='pagination-widget__page')) - 4
    if pages < 0:
        pages = 1
    return pages


def pars(html): # парсит страницу
    soup = BeautifulSoup(html, 'html.parser')
    return soup


def getCharact(parsedHtml, charName):
    parsed = parsedHtml.find_all('tr')
    for element in parsed:
        name = element.find('span')
        if name:
            if name.get_text() == charName:
                element = element.find_all('td')
                return element[1].get_text()
    return 'None'


def fillTable(table, parsedHtml, si):
    balls = 0
    sheet = table.get_sheet('Ноутбуки')

    charact = getCharact(parsedHtml, ' Разрешение экрана ')
    if charact == ' 1366x768':
        balls += 1
    elif charact == ' 1600x900':
        balls += 2
    elif charact == ' 1920x1080' or ' 1920x1200':
        balls += 3
    elif charact == ' 2560x1600':
        balls += 4
    sheet.write(si, 3, charact)

    charact = getCharact(parsedHtml, ' Производитель процессора ') + \
              getCharact(parsedHtml, ' Линейка процессора ') + getCharact(parsedHtml, ' Модель процессора ')
    sheet.write(si, 4, charact)

    charact = getCharact(parsedHtml, ' Количество ядер процессора ')
    if charact == ' 2':
        balls += 1
    elif charact == ' 4':
        balls += 2
    elif charact == ' 6':
        balls += 3
    elif charact == ' 8':
        balls += 4
    sheet.write(si, 5, charact)

    charact = getCharact(parsedHtml, ' Максимальное число потоков ')
    if charact != 'None':
        if int(charact) >= 16:
            balls += 5
        elif int(charact) >= 8:
            balls += 4
        elif int(charact) >= 6:
            balls += 3
        elif int(charact) >= 4:
            balls += 2
        else:
            balls += 1
    sheet.write(si, 6, charact)

    charact = getCharact(parsedHtml, ' Частота ')
    i = 1
    val = ''
    while charact[i] != ' ':
        val += charact[i]
        i += 1
    if float(val) >= 4:
        balls += 4
    elif float(val) >= 3:
        balls += 3
    elif float(val) >= 2:
        balls += 2
    elif float(val) >= 1:
        balls += 1
    sheet.write(si, 7, charact)

    charact = getCharact(parsedHtml, ' Автоматическое увеличение частоты ')
    if charact != ' нет':
        i = 1
        val = ''
        while charact[i] != ' ':
            val += charact[i]
            i += 1
        if float(val) >= 4:
            balls += 2
        elif float(val) >= 3:
            balls += 1
    sheet.write(si, 8, charact)

    charact = getCharact(parsedHtml, ' Кэш L2 ')
    i = 1
    val = ''
    while charact[i] != ' ':
        val += charact[i]
        i += 1
    if 'М' in charact:
        if int(val) >= 4:
            balls += 4
        elif int(val) >= 3:
            balls += 3
        elif int(val) >= 2:
            balls += 2
        elif int(val) >= 1:
            balls += 1
    sheet.write(si, 9, charact)

    charact = getCharact(parsedHtml, ' Кэш L3 ')
    if charact != ' нет':
        i = 1
        val = ''
        while charact[i] != ' ':
            val += charact[i]
            i += 1
        if 'М' in charact:
            if int(val) >= 4:
                balls += 4
            elif int(val) >= 3:
                balls += 3
            elif int(val) >= 2:
                balls += 2
            elif int(val) >= 1:
                balls += 1
    sheet.write(si, 10, charact)

    charact = getCharact(parsedHtml, ' Технологический процесс ')
    i = 1
    val = ''
    while charact[i] != ' ':
        val += charact[i]
        i += 1
    if int(val) >= 14:
        balls += 3
    elif int(val) >= 7:
        balls += 2
    else:
        balls += 1
    sheet.write(si, 11, charact)

    charact = getCharact(parsedHtml, ' Тип оперативной памяти ')
    if charact == ' DDR4':
        balls += 2
    elif charact == ' DDR3':
        balls += 1
    sheet.write(si, 12, charact)

    charact = getCharact(parsedHtml, ' Частота оперативной памяти ')
    if charact != 'None':
        i = 1
        val = ''
        while charact[i] != ' ':
            val += charact[i]
            i += 1
        if int(val) >= 4000:
            balls += 3
        elif int(val) >= 3000:
            balls += 2
        elif int(val) >= 2000:
            balls += 1
    sheet.write(si, 13, charact)

    charact = getCharact(parsedHtml, ' Размер оперативной памяти ')
    i = 1
    val = ''
    while charact[i] != ' ':
        val += charact[i]
        i += 1
    if int(val) >= 16:
        balls += 5
    elif int(val) >= 8:
        balls += 4
    elif int(val) >= 4:
        balls += 2
    sheet.write(si, 14, charact)

    charact = getCharact(parsedHtml, ' Количество слотов под модули памяти ')
    if charact != ' интегрирована':
        balls += 1
    sheet.write(si, 15, charact)

    charact = getCharact(parsedHtml, ' Максимальный объем памяти ')
    if charact != ' не добавляется':
        balls += 1
    sheet.write(si, 16, charact)

    charact = getCharact(parsedHtml, ' Вид графического ускорителя ')
    sheet.write(si, 17, charact)

    charact = getCharact(parsedHtml, ' Производитель видеочипа ')
    if getCharact(parsedHtml, ' Модель встроенной видеокарты ') == ' нет':
        charact += getCharact(parsedHtml, ' Модель дискретной видеокарты ')
    elif getCharact(parsedHtml, ' Модель дискретной видеокарты ') == ' нет':
        charact += getCharact(parsedHtml, ' Модель встроенной видеокарты ')
    else:
        charact += getCharact(parsedHtml, ' Модель дискретной видеокарты ') + 'и' + \
                   getCharact(parsedHtml, ' Модель встроенной видеокарты ')
        balls += 1
    sheet.write(si, 18, charact)

    charact = getCharact(parsedHtml, ' Тип видеопамяти ')
    if charact == 'GDDR6':
        balls += 3
    elif charact == 'GDDR5':
        balls += 1
    sheet.write(si, 19, charact)

    charact = getCharact(parsedHtml, ' Объем видеопамяти ')
    if charact != ' выделяется из оперативной':
        i = 1
        val = ''
        while charact[i] != ' ':
            val += charact[i]
            i += 1
        if int(val) >= 8:
            balls += 6
        elif int(val) >= 6:
            balls += 5
        elif int(val) >= 4:
            balls += 4
        else:
            balls += 3
    sheet.write(si, 20, charact)

    val = '0'
    if getCharact(parsedHtml, ' Общий объем жестких дисков (HDD) ') == ' нет':
        charact = getCharact(parsedHtml, ' Общий объем твердотельных накопителей (SSD) ')
        i = 1
        val = ''
        while charact[i] != ' ':
            val += charact[i]
            i += 1
        sheet.write(si, 21, ' SSD')
    elif getCharact(parsedHtml, ' Общий объем твердотельных накопителей (SSD) ') == ' нет':
        charact = getCharact(parsedHtml, ' Общий объем жестких дисков (HDD) ')
        i = 1
        val = ''
        while charact[i] != ' ':
            val += charact[i]
            i += 1
        sheet.write(si, 21, ' HDD')
    else:
        charact = getCharact(parsedHtml, ' Общий объем жестких дисков (HDD) ') + ' +' + \
                  getCharact(parsedHtml, ' Общий объем твердотельных накопителей (SSD) ')
        balls += 4
        sheet.write(si, 21, ' HDD + SSD')
    if int(val) >= 500:
        balls += 3
    sheet.write(si, 22, charact)

    charact = getCharact(parsedHtml, ' Веб-камера ')
    if charact == ' есть':
        balls += 1
    sheet.write(si, 23, charact)

    charact = getCharact(parsedHtml, ' Встроенный микрофон ')
    if charact == ' есть':
        balls += 1
    sheet.write(si, 24, charact)

    charact = getCharact(parsedHtml, ' Стандарт Wi-Fi ')
    sheet.write(si, 25, charact)

    charact = getCharact(parsedHtml, ' Приблизительное время автономной работы ')
    if charact != 'None':
        i = 1
        val = ''
        while charact[i] != ' ':
            val += charact[i]
            i += 1
        if int(val) >= 6:
            balls += 3
    sheet.write(si, 26, charact)

    charact = getCharact(parsedHtml, ' Цифровой блок клавиатуры ')
    if charact == ' есть':
        balls += 1
    sheet.write(si, 27, charact)

    sheet.write(si, 28, balls)

    return table



wb = makeTable()
ws = wb.get_sheet('Ноутбуки')
price = 'price=' + str(input('Введите нижний порог цены: ')) + '-' + str(input('Верхний порог цены: '))
addressList = getItems(price)
strIndex = 0
for address in addressList:
    if address == 'S':
        break
    print(address)
    strIndex += 1
    soup = pars(get_HTML(DNS + address))
    wb = fillTable(wb, soup, strIndex)
    ws.write(strIndex, 0, soup.find(class_='ui-link ui-link_black').get_text())
    ws.write(strIndex, 1, getPrice(soup))
    ws.write(strIndex, 29, DNS + address)

wb.save('table.xls')




































































# Усложненная версия задания
Дедлайн - 7 календарных дней. Решить задачу нужно на языке программирования C#. **Свое решение прикрепляйте в поле ответа ниже в виде ссылки либо архива с кодом.** Упорядочивание файлов в вашем решении и комментарии к коду помогут быстрее понять ваш ход решения и завершить проверку задания.

Синим цветом обозначены ключевые изменения в условии задания по сравнению со стандартной версией.

## **Требуется реализовать два приложения, используя .NET Core / .NET:**

- сервисное приложение, реализующее простой REST API, который предоставляет ресурс для создания задач на сканирование файлов в директории и получения статуса задачи по идентификатору
- утилиту, работающую из командной строки, отправляющую сервисному приложению команды на создание и просмотр состояния задач.
**В рамках задачи определено 3 типа "подозрительного" содержимого в файле:**

- файл с расширением .js, содержащий строку *<script>evil_script()</script>*
- любой файл, содержащий строку: *rm -rf %userprofile%\Documents*
- любой файл, содержащий строку: *Rundll32 sus.dll SusEntry*
**После завершения команды создание задачи на сканирование должен быть выведен уникальный идентификатор задачи.**

## **После завершения команды просмотра статуса задачи может быть выведено два результата:** статус "задача еще выполняется" или отчет о сканировании, в котором присутствует следующая информация:

- путь к директории, сканирование которой производилось
- общее количество обработанных файлов
- количество обнаружений на каждый тип "подозрительного" содержимого
- количество ошибок анализа файлов (например, не хватает прав на чтение файла)
- время выполнения утилиты.
## **Пример запуска сервиса и исполнения утилиты из командной строки:**

```
> scan_service
Scan service was started.
Press <Enter> to exit...

> scan_util scan %userprofile%\Documents
Scan task was created with ID: 1234

> scan_util status 1234
Scan task in progress, please wait

> scan_util status 1234
====== Scan result ======

Directory: C:\Users\TestUser\Documents
Processed files: 150
JS detects: 5
rm -rf detects: 1
Rundll32 detects: 2
Errors: 1
Exection time: 00:00:31
=========================
```
## **Примечание:**

- сервисное приложение не имеет постоянного хранилища состояния (каждый запуск как чистый лист)
- в каждом файле может присутствовать только один тип "подозрительного" содержимого
- сервисное приложение и утилита работают на одном и том же устройстве
- рекомендуется максимальное использования (утилизация) вычислительных ресурсов устройства, на котором выполняется утилита.

---

## Решение
К сожалению, из-за семейных обстоятельств, я не успел доделать сложную версию задания, однако у меня остался алгоритм и схема для него, поэтому закину сюда по крайней мере его, чтоб ветка не пустовала

## Сервер
Основной класс ```HttpServer``` ожидает подключения. При подключении к нему клиента, выделяется отдельный поток для подключения ```_connectionThread```, который обрабатывает подключение.

Класс ```ConnectionManager``` обрабатывает входящие запросы и объект Manager создаётся для каждого отдельного подключения
```ConnectionManager``` имеет поле Task_ID, которому присваивается ID потока, в котором обрабатывается соединение. Поскольку и клиент, и сервис запускаются на одном компьютере, коллизий ID получится таким образом избежать.

## Клиент
Основной класс ```HttpClient``` подключается по предварительно настроенному адресу к серверу и отправляет сначала "Заголовок", в котором находится полный путь к папке
URI: ```http://localhost:<port>/scan_util/&path=<path/to/dir>```
Сервер отправляет ответом Task_ID
Далее обработчик папок рекурсивно обходит все папки внутри и отправляет их севреру в JSON следующего вида
```
{
  Task_ID: <int>
  Files: [
      {
      Filename: <string> - полный путь к файлу
      Data: <string> - данные файла
      }
    ]
}
```


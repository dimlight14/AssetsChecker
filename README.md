# AssetsChecker
![image](https://github.com/dimlight14/AssetsChecker/assets/17859007/202076e1-8f6c-4a6d-a338-ee1f659907ae)


### Устройство работы.

Окно инструмента вызывается через _Tools=>Find Missing References_.

Раздел _Settings_ определяет какие папки и файлы будут исключены из поиска (Scripts, Packages, Scenes).

_Path Filter_ фильтрует результаты, показывая только ассеты, у которых в пути содержится введенное в фильтр слово.

Таблица ассетов:
* _#_  - номер ассета в списке
* _Asset path_  - путь до ассета в проекте. Нажатие на эту кнопку открывает местоположение ассета в Project tab.
* _Asset type_ - тип ассета. Все ошибки внутри одного префаба или сцены отображаются одной записью.
* _Marked Resolved_ - галочка для отображения ассетов, в которых пользователь избавился от ошибок. Не делает ничего, кроме изменения вида записи. Выставляется вручную по желанию. Добавлено для удобства навигации.
* _Mis. Ref. Count_ - количество ошибок внутри ассета (missing references + missing scripts + missing prefabs)
* _Field names or GameObject/Component/Name_ - имя поля с ошибкой или путь на сцене до объекта с ошибкой. Список, если есть несколько ошибок.

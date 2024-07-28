# AssetsChecker
![image](https://github.com/dimlight14/AssetsChecker/assets/17859007/202076e1-8f6c-4a6d-a338-ee1f659907ae)

A tool that scans a unity project and finds any missing references, missing script or missing prefab errors

### How to use:

Asset checker window can be found under _Tools=>Find Missing References_.

_Settings_ tab allows you to exclude certain folders from the search (Scripts, Packages, Scenes).

_Path Filter_ filters the results, showing only the assets whose project path contains certain text. 

Assets table:
* _Asset path_  - the project path to the asset. Clicking opens its location in the Project tab.
* _Asset type_ - the type of an asset. All errors found inside one asset are displayed by a single entry.
* _Marked Resolved_ - a checkbox to mark an asset as error free. It's there just so it's easier to separate the assets that you have worked on from the ones that still have errors inside.
* _Mis. Ref. Count_ - the number of errors inside the asset
* _Field names or GameObject/Component/Name_ - the name of the field containing an error or the scene path to the game object. If there are several errors - a list is displayed instead.

_________________________________________________________________________________________________________________________________________________________________________________________________

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

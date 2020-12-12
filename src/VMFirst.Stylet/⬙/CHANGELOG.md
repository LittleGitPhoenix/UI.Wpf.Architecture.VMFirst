# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
___

## 2.1.0 (2020-12-12)

### Updated

- Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider ~~2.0.0~~ → **2.1.0**

### Fixed

- `StyletViewManager`is now using the `IViewManager.ViewLoaded` event to forward any newly loaded view to `StyletViewManager.BindViewToModel`. Therefore Stylets **View.ActionTarget**  will be updated properly, even if Stylet itself didn't load the view. This was the case when the **Phoenix.UI.Wpf.Architecture.VMFirst.DialogProvider** was used to display a message box where the views where loaded without the knowledge of Stylet. 
___

## 2.0.1 (2020-12-08)

### Fixed

- The `StyletBootstrapper`no longer automatically registers types within an assembly. This fixed a`NoConstructorsFoundException` with the [ProcessedByFody class](<https://github.com/Fody/Home/blob/master/pages/processedbyfody-class.md>) when using [Fody](<https://github.com/Fody/Home>).
___

## 2.0.0 (2020-11-21)

### Changed

- Changed license to [**LGPL-3.0**](https://www.gnu.org/licenses/lgpl-3.0.html).
___

## 1.1.0 (2020-11-15)

### Added

- Now also targeting **.NET5.0**.
___

## 1.0.0 (2020-02-16)

- Initial release
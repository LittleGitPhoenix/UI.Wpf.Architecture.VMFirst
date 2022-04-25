# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
___

## 3.0.0 (2022-04-25)

### Added

- Project now natively supports **.NET 6**.
- New overridable entry points for `StyletBootstrapper` to configure **Autofacs** IoC container.
  - `StyletBootstrapper.RegisterServices`

- New overridable entry points for `StyletBootstrapper` with direct access to **Autofacs** IoC container.
  - `StyletBootstrapper.BeforeLaunch`
  - `StyletBootstrapper.AfterLaunch`
  - `StyletBootstrapper.OnClosing`

### Changed

- New minimum **.NET Framework** version is now **4.8**.
- The project now uses [nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references).
- The **Autofac** container is now a private member of `StyletBootstrapper`. This was done to prevent accidental usage of the [Service Locator anti-pattern](https://en.wikipedia.org/wiki/Service_locator_pattern).

### Fixed

- `StyletViewManager` did not set the **View** property of views implementing **Stylet.IViewAware**. This broke functionality of all **Stylet.Screen** implementations.

### Deprecated

- ~~`StyletBootstrapper.ConfigureIoC`~~ → use `StyletBootstrapper.RegisterServices`
- ~~`StyletBootstrapper.Configure`~~ → use `StyletBootstrapper.BeforeLaunch`
- ~~`StyletBootstrapper.OnLaunch`~~ → use `StyletBootstrapper.AfterLaunch`
- ~~`StyletBootstrapper.OnExit`~~ → use `StyletBootstrapper.OnClosing`

### Removed

- Dropped support for **.NET Framework 4.5**.

### References

:large_blue_circle: Phoenix.UI.Wpf.Architecture.VMFirst ~~2.1.0~~ → [**3.0.0**](../../VMFirst/⬙/CHANGELOG.md)
:large_blue_circle: Stylet ~~1.3.5~~ → **1.3.6**

___

## 2.1.0 (2020-12-12)

### Updated

- Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider ~~2.0.0~~ → [**2.1.0**](https://github.com/LittleGitPhoenix/UI.Wpf.Architecture.VMFirst.ViewProvider/blob/master/src/ViewProvider/%E2%AC%99/CHANGELOG.md)

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
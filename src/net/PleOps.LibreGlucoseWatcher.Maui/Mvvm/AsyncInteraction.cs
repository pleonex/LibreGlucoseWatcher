// Copyright (C) 2023  Benito Palacios Sánchez
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
namespace PleOps.LibreGlucoseWatcher.Maui.Mvvm;

public class AsyncInteraction<TInput, TOutput>
{
    private Func<TInput, Task<TOutput>>? _handler;

    public Task<TOutput> HandleAsync(TInput input)
    {
        if (_handler is null)
        {
            throw new InvalidOperationException("Missing handler");
        }

        return _handler(input);
    }

    public void RegisterHandler(Func<TInput, Task<TOutput>> handler)
    {
        _handler = handler;
    }
}

public class AsyncInteraction<TOutput>
{
    private Func<Task<TOutput>>? _handler;

    public Task<TOutput> HandleAsync()
    {
        if (_handler is null)
        {
            throw new InvalidOperationException("Missing handler");
        }

        return _handler();
    }

    public void RegisterHandler(Func<Task<TOutput>> handler)
    {
        _handler = handler;
    }
}

public class AsyncInteraction
{
    private Func<Task>? _handler;

    public Task HandleAsync()
    {
        if (_handler is null)
        {
            throw new InvalidOperationException("Missing handler");
        }

        return _handler();
    }

    public void RegisterHandler(Func<Task> handler)
    {
        _handler = handler;
    }
}

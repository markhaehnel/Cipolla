# Cipolla

> This repository is no longer maintained. Please use it's successor [cipolla-rs](https://github.com/markhaehnel/cipolla-rs) instead.

Runs multiple Tor SOCKS5 proxy instances on your machine.

## Installation

> `tor` needs to be installed and in your PATH environemnt variable.

### Linux

On Linux, first install `tor`

> More information is available in the [Tor Install Guide](https://community.torproject.org/onion-services/setup/install/)

```
sudo apt install tor
    -or-
sudo yum install tor
```

Then download the [pre-built binaries](https://github.com/markhaehnel/Cipolla/releases).

### macOS

On macOS, first `tor` using [Homebrew](https://brew.sh/) or [MacPorts](https://www.macports.org/)

> More information is available in the [Tor Install Guide](https://community.torproject.org/onion-services/setup/install/)

```
brew install
    -or-
sudo port install tor
```

Then download the [pre-built binaries](https://github.com/markhaehnel/Cipolla/releases).

### Windows

On Windows, first download the `Windows Export Bundle` from the [Tor Website](https://www.torproject.org/download/tor/) and put the directory containing `tor.exe` to your PATH.

Then download the [pre-built binaries](https://github.com/markhaehnel/Cipolla/releases).

## Usage

```bash
# Run with default values (10 proxies listening on port 9250 to 9259)
./cipolla
```

```bash
# Something not working? Run in verbose mode to get information to debug your issue
./cipolla -v
```

All options:

```
cipolla 0.1.0
Copyright (C) 2021 cipolla

  -n, --num-instances     (Default: 10) Number of Tor instances to launch

  -p, --socks-port        (Default: 9250) Starting port number for socks port

  -d, --data-directory    Starting port number for control port

  -i, --interval          (Default: 30) Instance check interval in seconds

  -v, --verbose           (Default: false) Makes logging output more verbose

  --help                  Display this help screen.

  --version               Display version information.
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

Created by Mark Hähnel and released under the terms of the [MIT](https://choosealicense.com/licenses/mit/)

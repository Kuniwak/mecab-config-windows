package main

import (
	"flag"
	"fmt"
	"os"
	"os/exec"
	"path/filepath"
	"runtime"
)

func main() {
	flags := flag.NewFlagSet("mecab-config", flag.ContinueOnError)
	flags.SetOutput(os.Stderr)
	flags.Usage = func() {
		_, _ = fmt.Fprintf(flags.Output(), "Usage: %s [OPTIONS]\n", os.Args[0])
		flags.PrintDefaults()
	}

	version := flags.Bool("version", false, "show MeCab version")
	dicdir := flags.Bool("dicdir", false, "show MeCab dictionary directory")
	libexecdir := flags.Bool("libexecdir", false, "show MeCab libexec directory")
	sysconfdir := flags.Bool("sysconfdir", false, "show MeCab sysconf directory")
	incDir := flags.Bool("inc-dir", false, "show MeCab include directory")
	cflags := flags.Bool("cflags", false, "show MeCab compile flags")
	libs := flags.Bool("libs", false, "show MeCab link flags")

	if runtime.GOOS != "windows" {
		_, _ = fmt.Fprintln(flags.Output(), "This is a Windows-only tool.")
		os.Exit(1)
	}

	if err := flags.Parse(os.Args[1:]); err != nil {
		os.Exit(1)
	}

	prefix, err := getPrefix()
	if err != nil {
		_, _ = fmt.Fprintln(os.Stderr, err.Error())
		os.Exit(1)
	}

	if *version {
		fmt.Println("0.996-custom")
		return
	}

	if *dicdir {
		fmt.Println(filepath.Join(prefix, "dic"))
		return
	}

	if *libexecdir {
		fmt.Println(filepath.Join(prefix, "bin"))
		return
	}

	if *sysconfdir {
		fmt.Println(filepath.Join(prefix, "etc"))
		return
	}

	if *incDir {
		fmt.Println(filepath.Join(prefix, "sdk"))
		return
	}

	if *cflags {
		fmt.Printf("-I%q\n", filepath.Join(prefix, "sdk"))
		return
	}

	if *libs {
		fmt.Printf("-L%q -lmecab -lstdc++\n", filepath.Join(prefix, "sdk"))
		return
	}

	flags.Usage()
	os.Exit(1)
}

func getPrefix() (string, error) {
	mecabPath, err := exec.LookPath("mecab")
	if err != nil {
		return "", err
	}

	binDir := filepath.Dir(mecabPath)
	prefix := filepath.Dir(binDir)
	return prefix, nil
}
